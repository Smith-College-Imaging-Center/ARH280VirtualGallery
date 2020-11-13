using UnityEngine;
using System;
using UnityEditor;

[CustomEditor(typeof(PrefabManager))]
[System.Serializable]
public class PrefabManagerCustomEditor: Editor
{
	public override void OnInspectorGUI()
	{
		var target_cs = (PrefabManager)target;
        DrawDefaultInspector();
		
		PrefabManager prefabManagerScript = target_cs;
        
		if(!Application.isPlaying)
		{
            if(GUILayout.Button("Create Prefabs"))
            {
                prefabManagerScript.CreatePrefabs();
            }
		}
	}
}