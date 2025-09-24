using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Level Editor/Tile Set/Object Tile Set")]
public class ObjectTileSet : ScriptableObject
{
    public GameObject[] objects;
}