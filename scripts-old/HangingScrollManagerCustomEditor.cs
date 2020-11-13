using UnityEngine;
using System;
using UnityEditor;

[CustomEditor(typeof(ARH280PrefabManager))]
[System.Serializable]
public class ARH280PrefabManagerCustomEditor: Editor
{
	public override void OnInspectorGUI()
	{
		var target_cs = (ARH280PrefabManager)target;
        DrawDefaultInspector();
		
		ARH280PrefabManager prefabManagerScript = target_cs;
        
		if(!Application.isPlaying)
		{
            if(GUILayout.Button("Create Prefabs"))
            {
                prefabManagerScript.CreatePrefabs();
            }
		}
	}
}