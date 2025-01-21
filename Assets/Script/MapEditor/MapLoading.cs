using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoading : MonoBehaviour
{
    [Header("Map Creator")]
    [SerializeField] int ColCount = 35;
    [SerializeField] int RowCount = 20;

    [SerializeField] GameObject gameTilePrefab;
    static internal GameTiles[,] currentGameTiles;
    internal Vector2 spawnPoint;
    internal Vector2 endPoint;
    static internal List<GameTiles> spawnTile = new List<GameTiles>();
    static internal GameTiles endTile;

    [SerializeField] List<GameObject> mapList = new List<GameObject>();
    static public GameObject MapObject;
    Map currentMap;

    [Header("Map loading")]
    [SerializeField] internal int mapIndex = 0;
    internal string mapName = "n/a";
    //string tagBackground = "Background";
    string tagMap = "Map";

    private void Awake()
    {
        CreateMap();
    }

    public void CreateMap()
    {
        RemoveMap();

        MapObject = mapList[mapIndex];
        Debug.LogWarning(MapObject.name);

        //mapList[mapIndex].GetComponent<Map>().LoadJson();

        //int currentRow = 0;
        //int currentCol = 0;

        //ColCount = mapList[mapIndex].GetComponent<Map>().GetCol();
        //RowCount = mapList[mapIndex].GetComponent<Map>().GetRow();

        //load l'image en background
        // Instancier le prefab dans la scène
        GameObject mapInstance = Instantiate(mapList[mapIndex]);

        //load le niveau
        currentMap = mapList[mapIndex].GetComponent<Map>();

        //-----------------------------------------------------------------------
        //pour modifier la taille de la map
        //changement de taille

        RowCount = currentMap.row;
        ColCount = currentMap.col;


        if (currentMap.row != RowCount && currentMap.col == ColCount)
        {
            Debug.LogWarning("Taille differente set une nouvel taille");
            //currentMap.row = RowCount;
            //currentMap.col = ColCount;

        }

        //------------------------------------------------------------------------

        currentMap.LoadJson();
        mapName = currentMap.mapName;
        //currentMap.prefab = MapObject;

        //changement de taille
        //if (currentMap.row == 0 && currentMap.col == 0)
        //{
        //    currentMap.row = RowCount;
        //    currentMap.col = ColCount;

        //}

        //currentRow = currentMap.row;
        //currentCol = currentMap.col;

        //RowCount = currentMap.row; 
        //ColCount = currentMap.col;

        //currentMap.row = RowCount;
        //currentMap.col = ColCount;

        if (currentMap.map == null)
        {
            currentMap.map = new char[RowCount, ColCount];
        }


        currentGameTiles = new GameTiles[RowCount, ColCount];

        for (int x = 0; x < ColCount; x++)
        {
            for (int y = 0; y < RowCount; y++)
            {

                var spawnPosition = new Vector3(x, 0, y);
                var tile = Instantiate(gameTilePrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
                tile.transform.parent = mapInstance.transform;
                currentGameTiles[y, x] = tile.GetComponent<GameTiles>();
                currentGameTiles[y, x].SetComponent();
                currentGameTiles[y, x].X = x;
                currentGameTiles[y, x].Y = y;

                //wall
                if (currentMap.map[y, x] == 'W')
                {
                    currentGameTiles[y, x].TurnBloced();
                }
                //Slowing Block
                else if (currentMap.map[y, x] == 'S')
                {
                    currentGameTiles[y, x].TurnSlow();
                }
                //Damaging Block
                else if (currentMap.map[y, x] == 'D')
                {
                    currentGameTiles[y, x].TurnDamaging();
                }
                //Spawn Tile
                else if (currentMap.map[y, x] == 'E')
                {
                    spawnPoint.x = spawnPosition.x;
                    spawnPoint.y = spawnPosition.z;
                    spawnTile.Add(currentGameTiles[y, x]);
                    currentGameTiles[y, x].TurnSpawn();
                }
                //end Tile
                else if (currentMap.map[y, x] == 'F')
                {
                    endPoint.x = spawnPosition.x;
                    endPoint.y = spawnPosition.z;
                    endTile = currentGameTiles[y, x];
                    currentGameTiles[y, x].TurnEnd();
                }

                //Debug.Log(currentMap.map[y, x]);
            }
        }
    }

    internal void RemoveMap()
    {

        mapName = "n/a";
        spawnPoint = Vector2.zero;
        endPoint = Vector2.zero;
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        //remove tiles
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile);
        }

        //remove map 
        foreach (var map in GameObject.FindGameObjectsWithTag(tagMap))
        {
            DestroyImmediate(map);
            Debug.Log($"map destroy");
        }

        PathFinder.pathToGoal.Clear();
        spawnTile.Clear();

    }

    internal void SaveMap()
    {
        mapList[mapIndex].GetComponent<Map>().SaveMap(currentGameTiles);
        RefreshMap();
    }

    internal void RefreshMap()
    {
        RemoveMap();
        CreateMap();
    }

    internal void ResizeMap()
    {
        foreach (var map in GameObject.FindGameObjectsWithTag(tagMap))
        {
            map.GetComponent<Map>().ResizeMap(RowCount, ColCount);
        }
    }

    internal void SetPath()
    {
        PathFinder pathFinder = FindAnyObjectByType<PathFinder>().GetComponent<PathFinder>();
        pathFinder.SetValue(currentGameTiles, spawnTile, endTile, ColCount, RowCount);
        pathFinder.SetPath();
    }

    internal void EditMap()
    {

    }

    internal int getRow()
    {
        return RowCount;
    }

    internal int getCol()
    {
        return ColCount;
    }
}