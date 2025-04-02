using UnityEngine;
using UnityEditor;
using static MapLoading;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;
using UnityEngine.U2D;

[CustomEditor(typeof(MapLoading))]
public class MapEditor : Editor
{
    public enum MapLayout
    {
        Base,
        Nature,
        Terrain,
        Decoration,
    }

    //public enum NatureLayout
    //{
    //    Plaine,
    //}

    private MapLoading mapLoading;
    bool showMap = true;
    GameTiles[,] currentMap;
    int[] colorIndex = new int[5];
    int currentColor; //0 = vide, 1 = block, 2 = water, 3 = fire, 4 = spawn, 5 = end

    //variable pour le tile set
    MapLayout mapLayer;
    //MapLayout terrainLayer;
    int pixelResolution = 32;
    int nombreParLigne = 9;
    //variable pour le layer Nature
    bool autoFill = false;
    GroupTileSet natureLayer;
    private int selectedSpriteIndex;

    private bool showSpawnPoints = false; //pour le dropdown pour l'affichage des spawn point

    private void OnEnable()
    {
        mapLoading = (MapLoading)target;
        mapLoading.CreateMap();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapLoading mapLoading = (MapLoading)target;

        #region map button
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Map Name : ", mapLoading.mapName);

        showSpawnPoints = EditorGUILayout.Foldout(showSpawnPoints, "Spawn Points");
        if (showSpawnPoints)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Liste des Spawn Points :");
            foreach (var spawn in mapLoading.GetSpawnPoint())
            {
                EditorGUILayout.LabelField("•", spawn.ToString());
            }

            EditorGUI.indentLevel--;
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
        #endregion

        #region map editor
        EditorGUILayout.LabelField("Grille de la Carte", EditorStyles.boldLabel);

        //dropdown pour la selection du layer de la map
        mapLayer = (MapLayout)EditorGUILayout.EnumPopup("Layer de la map", mapLayer);

        //si la selection du layer base est selectioner afficher cette selection
        //dans la selection base on peut venir modifier les information de la carte
        if (mapLayer == MapLayout.Base)
        {
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
        }

        //si la selection du layer base est selectioner afficher cette selection
        //dans la selection Nature permet de modifier le terrain de la carte par exemple forest/plaine, desert, volcon, chateau, etc
        if (mapLayer == MapLayout.Nature)
        {
            //assosi les valeur des variable
            natureLayer = mapLoading.gameTilePrefab.GetComponent<GameTiles>().natureLayer;

            //selection pour la map
            EditorGUILayout.BeginHorizontal();
            //buttun pour le auto fill ce qui permet de selectioner automatiquement quelle tuille sera afficher
            autoFill = EditorGUILayout.Toggle("AutoFill", autoFill);

            //trasforme les nom des Tile set dans le Group set pour selectionner le bon
            string[] tileSetNames = new string[natureLayer.groupSet.Length];

            for (int i = 0; i < natureLayer.groupSet.Length; i++)
            {
                tileSetNames[i] = natureLayer.groupSet[i].name;
            }

            //pop up pour la selection du Tile set
            mapLoading.natureLayerIndex = EditorGUILayout.Popup("Layer de la map : ",
                                                                mapLoading.natureLayerIndex,
                                                                tileSetNames);
            EditorGUILayout.EndHorizontal();

            if (!autoFill)
            {
                EditorGUILayout.LabelField("Tile Set selectioner : ", EditorStyles.boldLabel);

                //pour afficher les tiles a l'horizontal
                //affiche 10 tille par ligne
                int currentLigne = 0;
                EditorGUILayout.BeginHorizontal();

                for (int i = 0; i < natureLayer.groupSet[mapLoading.natureLayerIndex].tiles.Length - 1; i++)
                {
                    Sprite sprite = natureLayer.groupSet[mapLoading.natureLayerIndex].tiles[i];

                    Texture2D preview = AssetPreview.GetAssetPreview(sprite);

                    if (GUILayout.Button(preview != null ? preview : Texture2D.grayTexture,
                               GUILayout.Width(pixelResolution), GUILayout.Height(pixelResolution)))
                    {
                        selectedSpriteIndex = i;
                    }

                    currentLigne++;
                    if (currentLigne == nombreParLigne)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        currentLigne = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        //si la selection du layer Terrain est selectioner afficher cette selection
        //dans la selection Terrain on peut venir modifer le terrain de la map 
        if (mapLayer == MapLayout.Terrain)
        {
            //reset les valeurs pour la couleur
            currentColor = -1;
            //terrainLayer = (MapLayout)EditorGUILayout.EnumPopup("Layer de la map", mapLoading.tilesets);
            //mapLoading.CurrentTileSet = EditorGUILayout.EnumPopup("Layer de la map", mapLoading.tilesets);
            mapLoading.tilesets = (TileSets)EditorGUILayout.EnumPopup("Layer de la map", mapLoading.tilesets);
            EditorGUILayout.LabelField("Tile Set selectioner : ", EditorStyles.boldLabel);

            //pour afficher les tiles a l'horizontal
            //affiche 10 tille par ligne
            int currentLigne = 0;
            EditorGUILayout.BeginHorizontal();

            foreach (var sprite in mapLoading.CurrentTileSet.tiles)
            {
                if (sprite == null)
                { continue; }

                Texture2D preview = AssetPreview.GetAssetPreview(sprite);

                if (preview != null)
                {
                    GUILayout.Box(preview, GUILayout.Width(pixelResolution), GUILayout.Height(pixelResolution));
                }
                else
                {
                    GUILayout.Label("?", GUILayout.Width(pixelResolution), GUILayout.Height(pixelResolution));
                }

                currentLigne++;
                if (currentLigne == nombreParLigne)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    currentLigne = 0;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        //Permet D'afficher la map
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
                    switch (mapLayer)
                    {
                        case MapLayout.Base:
                            EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y]));
                            break;
                        case MapLayout.Nature:
                            if (currentMap[x, y].natureRenderer.sprite == null)
                            {
                                EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y]));
                            }
                            else
                            {
                                Texture2D texture = currentMap[x, y].natureRenderer.sprite.texture;
                                GUI.DrawTextureWithTexCoords(cellRect, texture, cellRect);
                            }
                            break;
                        case MapLayout.Terrain:
                            EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y]));
                            break;
                        case MapLayout.Decoration:
                            EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y]));
                            break;

                    }


                    // Vérifier le clic sur la case
                    if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) && cellRect.Contains(Event.current.mousePosition))

                    {
                        foreach (var tile in mapLoading.GetCurrentMap())
                        {
                            if (currentMap[x, y].gameObject == tile.gameObject)
                            {
                                // Marquer l'objet comme modifié pour Undo/Redo
                                Undo.RecordObject(mapLoading, "Change Tile State");

                                switch (mapLayer)
                                {
                                    case MapLayout.Base:
                                        // Modifier l'état de la tuile
                                        CycleTileState(tile);
                                        tile.SetTileRender();
                                        break;
                                    case MapLayout.Nature:
                                        if (autoFill)
                                        {
                                            tile.SetTileRenderNature(mapLoading.natureLayerIndex, autoFill, -1);
                                        }
                                        else
                                        {
                                            tile.SetTileRenderNature(mapLoading.natureLayerIndex, autoFill, selectedSpriteIndex);
                                        }
                                        break;
                                    case MapLayout.Terrain:
                                        break;
                                    case MapLayout.Decoration:
                                        break;
                                }

                                // Dire à Unity que l'objet a été modifié
                                EditorUtility.SetDirty(mapLoading);

                                // Rafraîchir l'affichage
                                Repaint();

                                // Bloquer l'événement pour éviter qu'il soit propagé
                                Event.current.Use();
                            }
                        }

                    }
                }
            }
        }
        #endregion
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


    //inutile mais je sais pas on garde
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
