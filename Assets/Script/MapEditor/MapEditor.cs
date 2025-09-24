using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine.U2D;

using static MapLoading;
//using System.Diagnostics;

[CustomEditor(typeof(MapLoading))]
public class MapEditor : Editor
{
    public enum MapLayout
    {
        Base,
        Nature,
        Terrain,
        Object,
        VegetationObstacle,
    }

    private MapLoading mapLoading;
    bool showMap = true;
    GameTiles[,] currentMap;
    int[] colorIndex = new int[5];
    int currentColor; //0 = vide, 1 = block, 2 = water, 3 = fire, 4 = spawn, 5 = end

    bool isToggled = false;

    //variable pour le tile set
    MapLayout mapLayer;
    //MapLayout terrainLayer;
    int pixelResolution = 64;
    int nombreParLigne = 6;
    //variable pour le layer Nature
    bool autoFill = false;
    GroupTileSet natureLayer;
    GroupTileSet terrainLayer;
    GroupObjectTileSet objectLayer;
    private int selectedSpriteIndex;
    private Vector2 scrollNatureSelection;
    private Vector2 scrollTerrainSelection;
    private Vector2 scrollDecoSelection;
    private Vector2 scrollVegetationObstacleSelection;
    private Vector2 scrollPosition;

    private bool showSpawnPoints = false; //pour le dropdown pour l'affichage des spawn point

    private void OnEnable()
    {
        mapLoading = (MapLoading)target;
        mapLoading.CreateMap();
    }

    public override void OnInspectorGUI()
    {
        pixelResolution = 64;
        //nombreParLigne = 20;

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
            //associ les valeur des variable
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

            #region Toggle Sup buttun
            // Changer la couleur du bouton selon l'état
            Color originalColor = GUI.color;
            GUI.color = isToggled ? Color.red : Color.white;
            // Toggle bouton
            isToggled = GUILayout.Toggle(isToggled, isToggled ? "ACTIF" : "Supprimer la tuile", "Button", GUILayout.Height(20));
            // Rétablir la couleur originale
            GUI.color = originalColor;
            #endregion

            if (isToggled)
            {
                selectedSpriteIndex = -1;
            }
            else if (!isToggled && autoFill)
            {
                selectedSpriteIndex = 1;

            }

            if (!autoFill)
            {
                EditorGUILayout.LabelField("Tile Set selectioner : ", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("box"); // ma boite
                scrollNatureSelection = EditorGUILayout.BeginScrollView(scrollNatureSelection);
                //pour afficher les tiles a l'horizontal
                EditorGUILayout.BeginHorizontal();

                //fonction pour afficher le tile map
                ShowTimeMap(objectLayer, mapLoading.natureLayerIndex);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }
        }

        //si la selection du layer Terrain est selectioner afficher cette selection
        //dans la selection Terrain on peut venir modifer le terrain de la map 
        if (mapLayer == MapLayout.Terrain)
        {
            //associ les valeur des variable
            terrainLayer = mapLoading.gameTilePrefab.GetComponent<GameTiles>().terrainLayer;

            //selection pour la map
            EditorGUILayout.BeginHorizontal();
            //buttun pour le auto fill ce qui permet de selectioner automatiquement quelle tuille sera afficher
            autoFill = EditorGUILayout.Toggle("AutoFill", autoFill);

            //trasforme les nom des Tile set dans le Group set pour selectionner le bon
            string[] tileSetNames = new string[terrainLayer.groupSet.Length];

            for (int i = 0; i < terrainLayer.groupSet.Length; i++)
            {
                tileSetNames[i] = terrainLayer.groupSet[i].name;
            }

            //pop up pour la selection du Tile set
            mapLoading.natureLayerIndex = EditorGUILayout.Popup("Layer de la map : ",
                                                                mapLoading.natureLayerIndex,
                                                                tileSetNames);
            EditorGUILayout.EndHorizontal();

            #region Toggle Sup buttun
            // Changer la couleur du bouton selon l'état
            Color originalColor = GUI.color;
            GUI.color = isToggled ? Color.red : Color.white;
            // Toggle bouton
            isToggled = GUILayout.Toggle(isToggled, isToggled ? "ACTIF" : "Supprimer la tuile", "Button", GUILayout.Height(20));
            // Rétablir la couleur originale
            GUI.color = originalColor;
            #endregion

            if (isToggled)
            {
                selectedSpriteIndex = -1;
            }
            else if (!isToggled && autoFill)
            {
                selectedSpriteIndex = 1;

            }

            if (!autoFill)
            {
                EditorGUILayout.LabelField("Tile Set selectioner : ", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("box"); // ma boite
                scrollTerrainSelection = EditorGUILayout.BeginScrollView(scrollTerrainSelection);
                //pour afficher les tiles a l'horizontal
                EditorGUILayout.BeginHorizontal();

                //fonction pour afficher le tile map
                ShowTimeMap(objectLayer, mapLoading.terrainLayerIndex);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }
        }


        //si la selection du layer Terrain est selectioner afficher cette selection
        //dans la selection Terrain on peut venir modifer le terrain de la map 
        if (mapLayer == MapLayout.Object)
        {
            // terrainLayer = mapLoading.gameTilePrefab.GetComponent<GameTiles>().terrainLayer;
            //associ les valeur des variable
            objectLayer = mapLoading.gameTilePrefab.GetComponent<GameTiles>().Object3DLayer;

            //selection pour la map
            EditorGUILayout.BeginHorizontal();
            //buttun pour le auto fill ce qui permet de selectioner automatiquement quelle tuille sera afficher
            autoFill = EditorGUILayout.Toggle("AutoFill", autoFill);

            //trasforme les nom des Tile set dans le Group set pour selectionner le bon
            string[] tileSetNames = new string[objectLayer.objectTileSet.Length];

            for (int i = 0; i < objectLayer.objectTileSet.Length; i++)
            {
                tileSetNames[i] = objectLayer.objectTileSet[i].name;
            }

            //pop up pour la selection du Tile set
            mapLoading.natureLayerIndex = EditorGUILayout.Popup("Layer de la map : ",
                                                                mapLoading.object3DLayerIndex,
                                                                tileSetNames);
            EditorGUILayout.EndHorizontal();

            #region Toggle Sup buttun
            // Changer la couleur du bouton selon l'état
            Color originalColor = GUI.color;
            GUI.color = isToggled ? Color.red : Color.white;
            // Toggle bouton
            isToggled = GUILayout.Toggle(isToggled, isToggled ? "ACTIF" : "Supprimer la tuile", "Button", GUILayout.Height(20));
            // Rétablir la couleur originale
            GUI.color = originalColor;
            #endregion

            if (isToggled)
            {
                selectedSpriteIndex = -1;
            }
            else if (!isToggled && autoFill)
            {
                selectedSpriteIndex = 1;

            }

            if (!autoFill)
            {
                EditorGUILayout.LabelField("Tile Set selectioner : ", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("box"); // ma boite
                scrollTerrainSelection = EditorGUILayout.BeginScrollView(scrollTerrainSelection);
                //pour afficher les tiles a l'horizontal
                EditorGUILayout.BeginHorizontal();

                //fonction pour afficher le tile map
                ShowTimeMap(objectLayer, mapLoading.object3DLayerIndex);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }

        }

        //Permet D'afficher la map
        if (showMap)
        {
            if (currentMap == null || currentMap != mapLoading.GetCurrentMap())
            {
                currentMap = mapLoading.GetCurrentMap();

            }


            //pour mettre la carte dans une boite pour scroll
            EditorGUILayout.BeginVertical("box"); // ma boite
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Taille des cases
            float cellSize = 20f;
            float padding = 2f;

            //Création de la grille
            Rect gridRect = GUILayoutUtility.GetRect(mapLoading.ColCount * (cellSize + padding), mapLoading.RowCount * (cellSize + padding));

            //boucle pour la carte
            for (int y = 0; y < mapLoading.ColCount; y++)
            {
                for (int x = 0; x < mapLoading.RowCount; x++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + x * (cellSize + padding),
                        gridRect.y + +(mapLoading.ColCount - 1 - y) * (cellSize + padding),
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
                            if (currentMap[x, y].terrainRenderer.sprite == null)
                            {
                                EditorGUI.DrawRect(cellRect, GetTileColor(currentMap[x, y]));
                            }
                            else
                            {
                                Texture2D texture = currentMap[x, y].terrainRenderer.sprite.texture;
                                GUI.DrawTextureWithTexCoords(cellRect, texture, cellRect);
                            }
                            break;
                        case MapLayout.Object:
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
                                            tile.SetTileRenderNature(mapLoading.natureLayerIndex, autoFill, selectedSpriteIndex);
                                        }
                                        else
                                        {
                                            tile.SetTileRenderNature(mapLoading.natureLayerIndex, autoFill, selectedSpriteIndex);
                                        }
                                        break;
                                    case MapLayout.Terrain:
                                        if (autoFill)
                                        {
                                            tile.SetTileRenderTerrain(mapLoading.terrainLayerIndex, autoFill, selectedSpriteIndex);
                                        }
                                        else
                                        {
                                            tile.SetTileRenderTerrain(mapLoading.terrainLayerIndex, autoFill, selectedSpriteIndex);
                                        }
                                        break;
                                    case MapLayout.Object:
                                        {
                                            tile.SetObjectTileRenderTerrain(mapLoading.object3DLayerIndex, autoFill, selectedSpriteIndex);
                                        }
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

            //fin de la boite et du scroll
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();


        }
        #endregion
        serializedObject.ApplyModifiedProperties();

    }

    private void ShowTimeMap(GroupObjectTileSet tileSet, int index)
    {
        int currentLigne = 0;
        //bloucle a traver toute les tuile pour l'afichage
        for (int i = 0; i < tileSet.objectTileSet[index].objects.Length - 1; i++)
        {
            GameObject objects = tileSet.objectTileSet[index].objects[i];

            Texture2D preview = AssetPreview.GetAssetPreview(objects);

            if (GUILayout.Button(preview != null ? preview : Texture2D.grayTexture,
                       GUILayout.Width(pixelResolution), GUILayout.Height(64)))
            {
                selectedSpriteIndex = i;
            }

            currentLigne++;
            //apres 6 tuile pase a la profaine ligne
            if (currentLigne == 6)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                currentLigne = 0;
            }
        }
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
