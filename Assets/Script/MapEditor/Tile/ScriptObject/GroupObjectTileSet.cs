using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Level Editor/Group Set/Group Object Tile Set")]
public class GroupObjectTileSet : ScriptableObject
{
    public ObjectTileSet[] objectTileSet;
}
