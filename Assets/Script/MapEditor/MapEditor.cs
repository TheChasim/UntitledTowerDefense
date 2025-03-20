using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLoading))]
public class MapEditor : Editor
{
    private MapLoading mapLoading;
    bool showMap = true;
    GameTiles[,] currentMap;
    int[] colorIndex = new int[5];
    int currentColor; //0 = vide, 1 = block, 2 = water, 3 = fire, 4 = spawn, 5 = end

    private void OnEnable()
    {
        mapLoading = (MapLoading)target;
        mapLoading.CreateMap();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapLoading mapLoading = (MapLoading)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Map Name : ", mapLoading.mapName);

        EditorGUILayout.LabelField("Spawn point : ");
        foreach (var spawn in mapLoading.GetSpawnPoint())
        {
            EditorGUILayout.LabelField(/*"Spawn point : ",S*/" ", spawn.ToString());
        }
        EditorGUILayout.LabelField("End point : ", mapLoading.endPoint.ToString());
        EditorGUILayout.Space();

        if (GUILayout.Button("Spawn Map"))
        {
            mapLoading.CreateMap();
            showMap = true;
            currentMap = mapLoading.GetCurrentMap();
        }

        if (GUILayout.Button("Save Map"))
        {
            mapLoading.SaveMap();
        }

        if (GUILayout.Button("Remove Map"))
        {
            mapLoading.RemoveMap();
            showMap = false;
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Set path"))
        {
            mapLoading.SetPath();
        }

        if (GUILayout.Button("Resize Map"))
        {
            mapLoading.ResizeMap();
        }
        serializedObject.Update();

        if (mapLoading.GetCurrentMap() == null)
        {
            EditorGUILayout.HelpBox("Aucune carte chargée. Cliquez sur 'Spawn Map' pour générer une carte.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grille de la Carte", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        //0 = vide (blanc), 1 = block (noir), 2 = water (blue), 3 = fire (rouge), 4 = spawn (orange), 5 = end (jaune)
        if (GUILayout.Button("vide"))
        {
            currentColor = 0;
        }

        if (GUILayout.Button("Block"))
        {
            currentColor = 1;
        }

        if (GUILayout.Button("Water"))
        {
            currentColor = 2;
        }

        if (GUILayout.Button("Fire"))
        {
            currentColor = 3;
        }

        EditorGUILayout.EndHorizontal();

        if (showMap)
        {
            if (currentMap == null || currentMap != mapLoading.GetCurrentMap())
            {
                currentMap = mapLoading.GetCurrentMap();

            }

            // Taille des cases
            float cellSize = 15f;
            float padding = 2f;

            //Création de la grille
            Rect gridRect = GUILayoutUtility.GetRect(mapLoading.ColCount * (cellSize + padding), mapLoading.RowCount * (cellSize + padding));

            for (int y = 0; y < mapLoading.ColCount; y++)
            {
                for (int x = 0; x < mapLoading.RowCount; x++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + (mapLoading.ColCount - 1 - y) * (cellSize + padding), // Inverser X pour corriger le miroir horizontal                   
                        gridRect.y + (mapLoading.RowCount - 1 - x) * (cellSize + padding),  // Inverser Y
                        cellSize,
                        cellSize);

                    // Dessiner la case
                    EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y]));


                    // Vérifier le clic sur la case
                    if (Event.current.type == EventType.MouseDrag && cellRect.Contains(Event.current.mousePosition))

                    {
                        foreach (var tile in mapLoading.GetCurrentMap())
                        {
                            if (currentMap[x, y].gameObject == tile.gameObject)
                            {
                                // Marquer l'objet comme modifié pour Undo/Redo
                                Undo.RecordObject(mapLoading, "Change Tile State");

                                // Modifier l'état de la tuile
                                //CycleTileState(currentMap[x, y]);
                                CycleTileState(tile);

                                // Dire à Unity que l'objet a été modifié
                                EditorUtility.SetDirty(mapLoading);

                                // Rafraîchir l'affichage
                                Repaint();

                                // Bloquer l'événement pour éviter qu'il soit propagé
                                Event.current.Use();

                                tile.SetTileRender();
                            }
                        }

                    }
                }
            }
        }

        EditorGUILayout.BeginHorizontal();

        //0 = vide (blanc), 1 = block (noir), 2 = water (blue), 3 = fire (rouge), 4 = spawn (orange), 5 = end (jaune)
        if (GUILayout.Button("Spawn"))
        {
            currentColor = 4;
        }

        if (GUILayout.Button("End"))
        {
            currentColor = 5;
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

    }

    // Définir la couleur en fonction du type de tile
    private Color GetTileColor(GameTiles cell)
    {
        if (cell.IsBloced) return Color.black; // Noir
        if (cell.IsSlowing) return Color.blue; // Bleu
        if (cell.IsDamaging) return Color.red; // Rouge
        if (cell.IsSpawn) return Color.green;
        if (cell.IsEnd) return Color.yellow;
        return Color.white; // Blanc (Normal)
    }

    // Change l'état du tile en cliquant dessus
    private void CycleTileState(GameTiles cell)
    {
        //0 = vide (blanc), 1 = block (noir), 2 = water (blue), 3 = fire (rouge), 4 = spawn (orange), 5 = end (jaune)
        switch (currentColor)
        {
            case 0:
                cell.IsBloced = false;
                cell.IsSlowing = false;
                cell.IsDamaging = false;
                cell.IsSpawn = false;
                cell.IsEnd = false;
                break;
            case 1:
                cell.IsBloced = true;
                cell.IsSlowing = false;
                cell.IsDamaging = false;
                cell.IsSpawn = false;
                cell.IsEnd = false;
                break;
            case 2:
                cell.IsBloced = false;
                cell.IsSlowing = true;
                cell.IsDamaging = false;
                cell.IsSpawn = false;
                cell.IsEnd = false;
                break;
            case 3:
                cell.IsBloced = false;
                cell.IsSlowing = false;
                cell.IsDamaging = true;
                cell.IsSpawn = false;
                cell.IsEnd = false;
                break;
            case 4:
                cell.IsBloced = false;
                cell.IsSlowing = false;
                cell.IsDamaging = false;
                cell.IsSpawn = true;
                cell.IsEnd = false;
                break;
            case 5:
                cell.IsBloced = false;
                cell.IsSlowing = false;
                cell.IsDamaging = false;
                cell.IsSpawn = false;
                cell.IsEnd = true;
                break;

        }
    }

    static void InvertRows(GameTiles[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        for (int i = 0; i < rows / 2; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                GameTiles temp = array[i, j];
                array[i, j] = array[rows - 1 - i, j];
                array[rows - 1 - i, j] = temp;
            }
        }
    }
}
