using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif 

public class Map : MonoBehaviour
{
    [SerializeField] internal GameObject prefab;
    [SerializeField] internal string mapName;
    //internal GameTiles[,] mapTile;
    internal char[,] map;

    [SerializeField] internal int col;
    [SerializeField] internal int row;
    //{
    //    {' ', ' ', ' ', ' ', 'X', 'X', 'X', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'X'}, //1
    //    {' ', ' ', ' ', ' ', ' ', 'X', 'X', ' ', ' ', ' ', ' ', 'X', ' ', ' ', ' ', 'X'}, //2
    //    {' ', 'X', ' ', ' ', ' ', 'X', 'X', ' ', 'X', ' ', ' ', 'X', ' ', ' ', ' ', 'X'}, //3
    //    {' ', ' ', 'X', ' ', ' ', ' ', 'X', ' ', 'X', ' ', ' ', ' ', ' ', ' ', ' ', ' '}, //4
    //    {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'X', ' ', ' ', 'F'}, //5
    //    {' ', 'X', ' ', ' ', ' ', ' ', 'X', ' ', ' ', 'X', ' ', ' ', 'X', ' ', ' ', ' '}, //6
    //    {' ', ' ', ' ', ' ', ' ', ' ', 'X', ' ', ' ', 'X', ' ', ' ', ' ', ' ', ' ', 'X'}, //7
    //    {'S', ' ', ' ', ' ', ' ', 'X', 'X', ' ', ' ', 'X', ' ', ' ', ' ', ' ', ' ', 'X'}, //8
    //    {' ', 'X', ' ', ' ', ' ', 'X', 'X', ' ', ' ', 'X', ' ', ' ', ' ', ' ', ' ', 'X'}, //9
    //    {' ', ' ', ' ', ' ', ' ', 'X', 'X', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'X'}  //10
    //    //  1    2    3    4    5    6    7   8     9    10   11  12    13   14   15   16
    //};

    public Map(GameObject prefab)
    {
        this.prefab = prefab;
    }
    public void SaveMap(GameTiles[,] CurrentMapTile)
    {
        col = map.GetLength(1);
        row = map.GetLength(0);


        map = new char[row,col];

        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                var tile = CurrentMapTile[y, x].GetComponent<GameTiles>();

                //Debug.Log($"Processing tile at ({y}, {x})");
                if (tile.IsBloced)
                {
                    map[y, x] = 'W';
                    Debug.Log("Wall");
                }
                else if (tile.IsDamaging)
                {
                    map[y, x] = 'D';
                    Debug.Log("Fire");
                }
                else if (tile.IsSlowing)
                {
                    map[y, x] = 'S';
                    Debug.Log("Water");
                }
                else if (tile.IsSpawn)
                {
                    map[y, x] = 'E';
                    Debug.Log("Spawing");
                }
                else if (tile.IsEnd)
                {
                    map[y, x] = 'F';
                    Debug.Log("End");
                }
                else
                {
                    map[y, x] = ' ';
                }
            }
        }

        SaveInPrefab();
    }


    private void SaveInPrefab()
    {
        var changedMap = MapLoading.MapObject.GetComponent<Map>();

        changedMap.map = this.map;
        changedMap.col = this.col;
        changedMap.row = this.row;

        EditorUtility.SetDirty(MapLoading.MapObject);
        PrefabUtility.SavePrefabAsset(MapLoading.MapObject);
        AssetDatabase.SaveAssets();
    }


    internal void ResetMap(int newRow, int newCol)
    {
        map = new char[row, col];

        this.row = newRow;
        this.col = newCol;
        map = null;
    }
}
