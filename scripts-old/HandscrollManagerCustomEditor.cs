using UnityEngine;
using System;
using UnityEditor;

[CustomEditor(typeof(ARH280HandscrollManager))]
[System.Serializable]
public class HandscrollManagerCustomEditor: Editor
{
	public override void OnInspectorGUI()
	{
		var target_cs = (ARH280HandscrollManager)target;
        DrawDefaultInspector();
		
		ARH280HandscrollManager handscrollManagerScript = target_cs;
        
		if(!Application.isPlaying)
		{
            if(GUILayout.Button("Create Handscrolls"))
            {
                string path = EditorUtility.OpenFolderPanel("Choose textures folder", "", "");
                Debug.Log(path);
                handscrollManagerScript.CreateHandscrolls();
            }
		}
	}
}