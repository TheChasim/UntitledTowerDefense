using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapViewEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    DrawDefaultInspector();

    //    Map map = (Map)target;
    //    //char[,] map = holder.map;

    //    // Afficher les champs de dimension
    //    map.row = EditorGUILayout.IntField("Rows", map.row);
    //    map.col = EditorGUILayout.IntField("Columns", map.col);

    //    // Réinitialiser le tableau si la taille change
    //    if (GUILayout.Button("Resize Array"))
    //    {
    //        map.map = new char[map.row, map.col];
    //    }

    //    // Afficher le tableau
    //    for (int i = 0; i < map.row; i++)
    //    {
    //        EditorGUILayout.BeginHorizontal();
    //        for (int j = 0; j < map.col; j++)
    //        {
    //            string value = EditorGUILayout.TextField(map.map[i, j].ToString());
    //            if (!string.IsNullOrEmpty(value))
    //            {
    //                map.map[i, j] = value[0];
    //            }
    //        }
    //        EditorGUILayout.EndHorizontal();
    //    }

    //    // Appliquer les changements
    //    if (GUI.changed)
    //    {
    //        EditorUtility.SetDirty(target);
    //    }
    //}
}
