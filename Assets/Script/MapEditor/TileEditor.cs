using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GameTiles))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameTiles tile = (GameTiles)target;


        if (GUILayout.Button("Wall"))
        {
            ApplyToAllTargets((GameTiles tile) => tile.TurnBloced());
        }

        if (GUILayout.Button("Damage"))
        {
            ApplyToAllTargets((GameTiles tile) => tile.TurnDamaging());
        }

        if (GUILayout.Button("Slowing"))
        {
            ApplyToAllTargets((GameTiles tile) => tile.TurnSlow());
        }

    }

    private void ApplyToAllTargets(System.Action<GameTiles> action)
    {
        foreach (var targetObj in targets)
        {
            GameTiles tile = (GameTiles)targetObj;
            action(tile);

            // Marquer la scène comme modifiée pour que les changements soient enregistrés
            EditorUtility.SetDirty(tile);
        }
    }
}
