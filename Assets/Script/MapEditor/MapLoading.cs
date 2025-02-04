using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoading : MonoBehaviour
{
    [Header("Map Creator")]
    [SerializeField] internal int ColCount = 35;
    [SerializeField] internal int RowCount = 20;

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
        Debug.LogWarning("Spawnig Map : " + MapObject.name);

        //load l'image en background
        // Instancier le prefab dans la scène
        GameObject mapInstance = Instantiate(mapList[mapIndex]);
        currentMap = mapList[mapIndex].GetComponent<Map>();


        //vient chercher les valeur de dimention de la map a spawn
        RowCount = currentMap.row;
        ColCount = currentMap.col;

        //cree un nouveau tableau pour la taille de la map
        currentMap.map = new char[RowCount, ColCount];

        //load le niveau
        currentMap.LoadJson();
        mapName = currentMap.mapName;



        currentGameTiles = new GameTiles[RowCount, ColCount];

        for (int x = 0; x < RowCount; x++)
        {
            for (int y = 0; y < ColCount; y++)
            {

                var spawnPosition = new Vector3(x, 0, y);
                var tile = Instantiate(gameTilePrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
                tile.transform.parent = mapInstance.transform;
                currentGameTiles[x, y] = tile.GetComponent<GameTiles>();
                currentGameTiles[x, y].SetComponent();
                currentGameTiles[x, y].X = x;
                currentGameTiles[x, y].Y = y;
                currentGameTiles[x, y].name = $"tile {x}/{y}";

                //wall
                if (currentMap.map[x, y] == 'W')
                {
                    currentGameTiles[x, y].TurnBloced();
                }
                //Slowing Block
                else if (currentMap.map[x, y] == 'S')
                {
                    currentGameTiles[x, y].TurnSlow();
                }
                //Damaging Block
                else if (currentMap.map[x, y] == 'D')
                {
                    currentGameTiles[x, y].TurnDamaging();
                }
                //Spawn Tile
                else if (currentMap.map[x, y] == 'E')
                {
                    spawnPoint.x = spawnPosition.x;
                    spawnPoint.y = spawnPosition.z;
                    spawnTile.Add(currentGameTiles[y, x]);
                    currentGameTiles[x, y].TurnSpawn();
                }
                //end Tile
                else if (currentMap.map[x, y] == 'F')
                {
                    endPoint.x = spawnPosition.x;
                    endPoint.y = spawnPosition.z;
                    endTile = currentGameTiles[x, y];
                    currentGameTiles[x, y].TurnEnd();
                }
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
        currentMap.ResizeMap(RowCount, ColCount);
        RefreshMap();
    }

    internal void SetPath()
    {
        //PathFinder pathFinder = FindAnyObjectByType<PathFinder>().GetComponent<PathFinder>();
        //pathFinder.SetValue(currentGameTiles, spawnTile, endTile, ColCount, RowCount);
        //pathFinder.SetPath();

        //NewPathFinder newPathFinder = FindAnyObjectByType<NewPathFinder>().GetComponent<NewPathFinder>();
        //newPathFinder.SetValue(currentGameTiles, spawnTile, endTile, ColCount, RowCount);
        //newPathFinder.FindPathAStar();

        FlowFieldPathfinding flowFieldPathfinding = FindAnyObjectByType<FlowFieldPathfinding>().GetComponent<FlowFieldPathfinding>();
        flowFieldPathfinding.SetFlowField(currentGameTiles);
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