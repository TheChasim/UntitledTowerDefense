using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif 

[System.Serializable]
public class MapData
{
    public string mapName;
    //public List<List<char>> mapToJson;
    public CharArray2DContainer mapComtainer;
    public int col;
    public int row;
    public char[,] map;
    public List<char> mapTo = new List<char>();
}


[Serializable]
public class CharArray2D
{
    public char[] row;
}

[Serializable]
public class CharArray2DContainer
{
    public CharArray2D[] map2D;
}

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
    //[SerializeField] internal List<List<char>> mapToJson;

    private void Awake()
    {
        path = Application.persistentDataPath + $"/{gameObject.name}.json";
    }

    private void Reset()
    {
        path = Application.persistentDataPath + $"/{gameObject.name}.json";
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
        List<char> list = new List<char>();
        list.Add('A');
        list.Add('a');
        list.Add('A');

        List<List<char>> deuxDimensions = new List<List<char>>();

        // Ajouter la première liste à deux dimensions
        deuxDimensions.Add(new List<char> { 'a', 'b', 'c' });

        // Ajouter la deuxième liste à deux dimensions
        deuxDimensions.Add(new List<char> { 'd', 'e', 'f' });

        //transforme la map pour le json
        CharArray2DContainer container = new CharArray2DContainer();
        container.map2D = new CharArray2D[map.GetLength(0)];

        for (int i = 0; i < map.GetLength(0); i++)
        {
            container.map2D[i] = new CharArray2D();
            container.map2D[i].row = new char[map.GetLength(1)];

            for (int j = 0; j < map.GetLength(1); j++)
            {
                container.map2D[i].row[j] = map[i, j];
            }
        }

        MapData mapData = new MapData
        {
            mapName = this.mapName,
            //map = this.map,
            //mapToJson = /*ConvertToSerializable(map),*/ deuxDimensions,
            col = this.col,
            row = this.row,
            mapTo = list,
            mapComtainer = container,
          
        };

        string json = JsonUtility.ToJson(mapData, true);

        // Sauvegarder le JSON dans un fichier

        if (path == "")
        {
            path = Application.persistentDataPath + $"/{gameObject.name}.json";
            Debug.Log("construc not working");
        }

        File.WriteAllText(path, json);

        Debug.Log("Data saved to " + path);
    }


    internal void LoadJson()
    {
        if(!File.Exists(path))
        {
            path = Application.persistentDataPath + $"/{gameObject.name}.json";
        }


        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);


            MapData mapData = JsonUtility.FromJson<MapData>(json);

            this.mapName = mapData.mapName;
            //this.map = mapData.map;
            this.row = mapData.row;
            this.col = mapData.col;
            //this.map = ConvertFromSerializable(mapData.mapToJson);

            map = new char[row, col];

            for(int i =0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    map[i,j] = mapData.mapComtainer.map2D[i].row[j];
                }

            }

            //if (map == null)
            //{
            //    map = new char[row, col];
            //}    



            Debug.Log("Data load from " + path);
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
        }
    }

    //private void SaveInPrefab()
    //{
    //    var changedMap = MapLoading.MapObject.GetComponent<Map>();

    //    changedMap.map = this.map;
    //    changedMap.col = this.col;
    //    changedMap.row = this.row;

    //    EditorUtility.SetDirty(MapLoading.MapObject);
    //    PrefabUtility.SavePrefabAsset(MapLoading.MapObject);
    //    AssetDatabase.SaveAssets();
    //}


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
        Debug.Log(list.Count);  
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
