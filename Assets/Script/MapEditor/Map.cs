using System;
using UnityEngine;
//using System.Runtime.Serialization.Json;
//using Unity.Plastic.Newtonsoft.Json;

using System.IO;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.Playables;






#if UNITY_EDITOR
using UnityEditor;
#endif 

[System.Serializable]
public class Map : MonoBehaviour
{
    //variable
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

    //Json Variable
    private string path;
    internal List<List<char>> mapToJson;

    public Map(GameObject prefab)
    {
        this.prefab = prefab;
        path = Application.persistentDataPath + $"/{gameObject.name}.json";
        LoadJson();
    }
    public void SaveMap(GameTiles[,] CurrentMapTile)
    {
        col = map.GetLength(1);
        row = map.GetLength(0);


        map = new char[row, col];

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



        SaveInJson(this, path);
        //SaveInPrefab();
    }



    private void SaveInJson<t>(t objectTosave, string destination)
    {
        mapToJson = ConvertToSerializable(map);
        string json = JsonUtility.ToJson(this);

        // Sauvegarder le JSON dans un fichier

        if (path == "")
        {
            path = Application.persistentDataPath + $"/{gameObject.name}.json";
            Debug.Log("construc not working");
        }

        File.WriteAllText(path, json);

        Debug.Log("Data saved to " + path);
    }


    private void LoadJson()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            // Charger le JSON depuis le fichier
            Map map = JsonUtility.FromJson<Map>(json);

            this.map = ConvertFromSerializable(mapToJson);
            this.name = map.name;
            this.row = map.row;
            this.col = map.col;
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
        }
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

    List<List<char>> ConvertToSerializable(char[,] array)
    {
        List<List<char>> list = new List<List<char>>();
        for (int i = 0; i < array.GetLength(0); i++)
        {
            List<char> row = new List<char>();
            for (int j = 0; j < array.GetLength(1); j++)
            {
                row.Add(array[i, j]);
            }
            list.Add(row);
        }
        return list;
    }

    char[,] ConvertFromSerializable(List<List<char>> list)
    {
        int rows = list.Count;
        int cols = list[0].Count;
        char[,] array = new char[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = list[i][j];
            }
        }
        return array;
    }
}
