using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLoading))]
public class MapEditor : Editor
{
    private MapLoading mapLoading;
    bool showMap = true;
    GameTiles[,] currentMap;

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
            InvertRows(currentMap);
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


        #region
        //serializedObject.Update();

        //if (mapLoading.GetCurrentMap() != null && showMap)
        //{
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField("Grid Cells", EditorStyles.boldLabel);


        //    for (int x = 0; x < mapLoading.RowCount; x++)
        //    {
        //        EditorGUILayout.BeginHorizontal();
        //        for (int y = 0; y < mapLoading.ColCount; y++)
        //        {
        //            GameTiles cell = mapLoading.GetCurrentMap()[x, y];

        //            EditorGUILayout.BeginVertical(GUILayout.Width(50));
        //            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        //            if (cell.IsBloced)
        //            {
        //                buttonStyle.normal.background = MakeTex(16, 16, Color.black);
        //            }
        //            else if (cell.IsSlowing)
        //            {
        //                buttonStyle.normal.background = MakeTex(16, 16, Color.blue);
        //            }
        //            else if (cell.IsDamaging)
        //            {
        //                buttonStyle.normal.background = MakeTex(16, 16, Color.red);
        //            }
        //            else
        //            {
        //                buttonStyle.normal.background = MakeTex(16, 16, Color.white);
        //            }

        //            GUILayout.Button("", buttonStyle, GUILayout.Width(30), GUILayout.Height(30));

        //            EditorGUILayout.EndVertical();
        //        }
        //        EditorGUILayout.EndHorizontal();
        //    }
        //}

        #endregion
        serializedObject.Update();

        if (mapLoading.GetCurrentMap() == null)
        {
            EditorGUILayout.HelpBox("Aucune carte chargée. Cliquez sur 'Spawn Map' pour générer une carte.", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grille de la Carte", EditorStyles.boldLabel);
        if (showMap)
        {
            // Taille des cases
            float cellSize = 15f;
            float padding = 2f;


            //Création de la grille
            Rect gridRect = GUILayoutUtility.GetRect(mapLoading.ColCount * (cellSize + padding), mapLoading.RowCount * (cellSize + padding));

            for (int y = 0; y < mapLoading.ColCount; y++  /*int x = mapLoading.RowCount - 1; x >= 0; x--*/)
            {
                for (int x = 0; x < mapLoading.RowCount; x++ /*int x = mapLoading.RowCount - 1; x >= 0; x--*/)
                {

                    Rect cellRect = new Rect(
                        //gridRect.x + x * (cellSize + padding),
                        //gridRect.y + y * (cellSize + padding),
                        gridRect.x + (mapLoading.ColCount - 1 - y) * (cellSize + padding), // Inverser X pour corriger le miroir horizontal
                        gridRect.y + x * (cellSize + padding), // Ne pas inverser Y pour correspondre à l'affichage du jeu
                        cellSize,
                        cellSize
                    );

                    // Dessiner la case
                    EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y] /*mapLoading.GetCurrentMap()[x,y]*/));

                    // Vérifier le clic sur la case
                    if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        //mapLoading.GetCurrentMap()[x, y].cellColor = GetNextColor(mapLoading.grid[x, y].cellColor);
                        //Event.current.Use(); // Empêche d'autres interactions
                    }
                }
            }

            //for (int y = 0; y < mapLoading.ColCount; y++)
            //{
            //    for (int x = mapLoading.RowCount - 1; x >= 0; x--) // Inversion de l'affichage des lignes
            //    {
            //        Rect cellRect = new Rect(
            //            gridRect.x + y * (cellSize + padding),
            //            gridRect.y + (mapLoading.RowCount - 1 - x) * (cellSize + padding), // Corrige l'orientation verticale
            //            cellSize,
            //            cellSize
            //        );

            //        EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y]));

            //        if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
            //        {
            //            // Gestion du clic sur la case
            //        }
            //    }
            //}
        }

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
        if (cell.IsBloced)
        {
            cell.IsBloced = false;
            cell.IsSlowing = true;
        }
        else if (cell.IsSlowing)
        {
            cell.IsSlowing = false;
            cell.IsDamaging = true;
        }
        else if (cell.IsDamaging)
        {
            cell.IsDamaging = false;
        }
        else
        {
            cell.IsBloced = true;
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
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
