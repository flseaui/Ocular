// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic;

[CanEditMultipleObjects]
[CustomEditor(typeof(HeightFogPerObject))]
public class HeightFogPerObjectInspector : Editor
{
	//private ADSGlobalMotion targetScript;

    private string[] excludeProps = new string[] { "m_Script"};

    private Color bannerColor;
    private string bannerText;
    private string helpURL;

    void OnEnable()
    {
        //targetScript = (ADSGlobalMotion)target;
        bannerColor = new Color(0.474f, 0.709f, 0.901f);
        bannerText = "Height Fog Per Object";
        helpURL = "https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.pzat2b29j9a0";
	}

	public override void OnInspectorGUI()
    {
        BEditorGUI.DrawBanner(bannerColor, bannerText, helpURL); 

        DrawInspector();
        //DrawWarnings ();
        //BEditorGUI.DrawLogo();
	}

	void DrawInspector()
    {
		serializedObject.Update ();

        excludeProps = new string[] { "m_Script"};

        DrawPropertiesExcluding(serializedObject, excludeProps);

        serializedObject.ApplyModifiedProperties ();

		GUILayout.Space (20);
	} 

//	void DrawWarnings(){
//
//	}
}
