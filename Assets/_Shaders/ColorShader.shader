Shader "Custom/ColorShader"
{
    Properties
    {
        [PerRendererData]_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [PerRendererData]_Contrast ("Contrast", Range(0,1)) = 1.0
        [PerRendererData]_Intensity ("Intensity", Range(0,3)) = 1.0
        [PerRendererData]_ShadowStrength ("ShadowStrength", Range(0, 1)) = 0.3
        [PerRendererData]_ShadowTint ("ShadowTint", Color) = (0.5, 0.5, 0.5, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Stepped fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        
        float3 _ShadowTint;
        
        half _Glossiness;
        half _Metallic;
        half _Intensity;
        half _Contrast;
        half _ShadowStrength;
        fixed4 _Color;
        
        //our lighting function. Will be called once per light
        float4 LightingStepped(SurfaceOutput s, float3 lightDir, half3 viewDir, float shadowAttenuation){
            //how much does the normal point towards the light?
            float towardsLight = dot(s.Normal, lightDir);
            // make the lighting a hard cut
            float towardsLightChange = fwidth(towardsLight);
            float lightIntensity = smoothstep(0, towardsLightChange, towardsLight) * .5;

        #ifdef USING_DIRECTIONAL_LIGHT
            //for directional lights, get a hard vut in the middle of the shadow attenuation
            float attenuationChange = fwidth(shadowAttenuation) * 0.5;
            float shadow = smoothstep(0.5 - attenuationChange, 0.5 + attenuationChange, shadowAttenuation);
        #else
            //for other light types (point, spot), put the cutoff near black, so the falloff doesn't affect the range
            float attenuationChange = fwidth(shadowAttenuation);
            float shadow = smoothstep(0, attenuationChange, shadowAttenuation);
        #endif
            lightIntensity = lightIntensity * shadow;

            //calculate shadow color and mix light and shadow based on the light. Then taint it based on the light color
            float3 shadowColor = s.Albedo * _ShadowTint;
            float4 color;
            color.rgb = lerp(shadowColor, s.Albedo, lightIntensity) * _LightColor0.rgb * _ShadowStrength;
            color.a = s.Alpha;
            return color;
        }


        struct Input
        {
            float2 uv_MainTex;
        };

        half3 AdjustContrast(half3 color, half contrast) {
            #if !UNITY_COLORSPACE_GAMMA
                color = LinearToGammaSpace(color);
            #endif
                color = saturate(lerp(half3(0.5, 0.5, 0.5), color, contrast));
            #if !UNITY_COLORSPACE_GAMMA
                color = GammaToLinearSpace(color);
            #endif
                return color;
        }


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            _Color += .5;
            fixed4 main = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = AdjustContrast(main.rgb, _Contrast) * _Color * _Intensity;
            o.Alpha = main.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
