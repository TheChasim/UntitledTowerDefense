using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLoading))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapLoading mapLoading = (MapLoading)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Map Name : ", mapLoading.mapName);
        EditorGUILayout.LabelField("Spawn point : ", mapLoading.spawnPoint.ToString());
        EditorGUILayout.LabelField("End point : ", mapLoading.endPoint.ToString());
        EditorGUILayout.Space();

        if (GUILayout.Button("Spawn Map"))
        {
            mapLoading.CreateMap();
        }

        if (GUILayout.Button("Save Map"))
        {
            mapLoading.SaveMap();
        }

        //if (GUILayout.Button("Edit Map"))
        //{
        //    mapLoading.EditMap();
        //}

        if (GUILayout.Button("Remove Map"))
        {
            mapLoading.RemoveMap();
        }

        EditorGUILayout.Space();
        if(GUILayout.Button("Set path"))
        {
            mapLoading.SetPath();
        }

        if (GUILayout.Button("Resize Map"))
        {
            mapLoading.ResizeMap();
        }
    }
}
