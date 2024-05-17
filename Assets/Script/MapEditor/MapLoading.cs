using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoading : MonoBehaviour
{
    const int ColCount = 35;
    const int RowCount = 20;

    [SerializeField] GameObject gameTilePrefab;
    static GameTiles[,] currentGameTiles;

    [SerializeField] List<GameObject> mapList = new List<GameObject>();
    Map currentMap;
    [SerializeField] internal int mapIndex = 0;
    internal string mapName = "n/a";
    string tagBackground = "Background";
    string tagMap = "Map";

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CreateMap();
        }
    }

    public void CreateMap()
    {
        RemoveMap();

        //load l'image en background
        // Instancier le prefab dans la scène
        GameObject mapInstance = Instantiate(mapList[mapIndex]);

        //load le niveau
        currentMap = mapList[mapIndex].GetComponent<Map>();
        mapName = currentMap.name;

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
                var tile = Instantiate(gameTilePrefab, spawnPosition, Quaternion.Euler(90,0,0));
                tile.transform.parent = mapInstance.transform;
                currentGameTiles[y, x] = tile.GetComponent<GameTiles>();
                currentGameTiles[y, x].SetComponent();

                if (currentMap.map[y, x] == 'W')
                {
                    currentGameTiles[y, x].TurnBloced();
                }    
                else if (currentMap.map[y, x] == 'S')
                {
                    currentGameTiles[y, x].TurnSlow();
                }
                else if (currentMap.map[y, x] == 'D')
                {
                    currentGameTiles[y,x].TurnDamaging();
                }

                Debug.Log(currentMap.map[y, x]);
            }
        }
    }

    internal void RemoveMap()
    {
        mapName = "n/a";
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
}
