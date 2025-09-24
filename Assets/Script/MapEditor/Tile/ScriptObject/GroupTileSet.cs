using UnityEngine;

[CreateAssetMenu(fileName = "NewTileSet", menuName = "Level Editor/Group Set/Group Tile Set")]
public class GroupTileSet : ScriptableObject
{
    public TileSet[] groupSet;
}
