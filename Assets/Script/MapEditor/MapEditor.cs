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
        EditorGUILayout.Space();

        if (GUILayout.Button("Spawn Map"))
        {
            mapLoading.CreateMap();
        }

        if (GUILayout.Button("Save Map"))
        {
            mapLoading.SaveMap();
        }

        if (GUILayout.Button("Remove Map"))
        {
            mapLoading.RemoveMap();
        }

        //if(GUILayout.Button("Reset Map"))
        //{
        //    mapLoading.ResetMap();
        //}
    }
}
