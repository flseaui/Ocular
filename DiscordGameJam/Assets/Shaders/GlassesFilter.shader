Shader "Hidden/GlassesFilter" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		[MaterialToggle] _RedFilter("Red Filter", Float) = 0
		[MaterialToggle] _GreenFilter("Green Filter", Float) = 0
		[MaterialToggle] _BlueFilter("Blue Filter", Float) = 0
	}

	SubShader {
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			float _RedFilter;
			float _GreenFilter;
			float _BlueFilter;

			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				col.rgb = col.rgb * float3(_RedFilter, _GreenFilter, _BlueFilter);
				return col;
			}
			ENDCG
		}
	}
}
