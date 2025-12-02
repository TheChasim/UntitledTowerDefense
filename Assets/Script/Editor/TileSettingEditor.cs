using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSetting))]
public class TileSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TileSetting tile = (TileSetting)target;

        //if (GUILayout.Button("Not Editable"))
        //{
        //    tile.OnChangeEditable();
        //}

        //if(GUILayout.Button("Hide in Editor"))
        //{
        //    tile.OnchangeHide();
        //}

        if(GUILayout.Button("Reset"))
        {
            tile.OnReset();
        }

    }
}
