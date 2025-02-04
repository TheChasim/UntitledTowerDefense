using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    GameTiles tiles;
    
    Vector3 worldPosition;
    int gridX, gridY;
    [Header("Cout de deplacement")]
    [SerializeField] internal float normalCost = 1;
    [SerializeField] internal float damageCost = 3;
    [SerializeField] internal float slowingCost = 1.5f;
    internal float cost // Coût de déplacement
    {
        get
        {
            if(tiles.IsSlowing)
            { return slowingCost;}
            if(tiles.IsDamaging)
            { return damageCost;}
            if(tiles.IsBloced) 
            { return float.MaxValue;}
            else 
            { return damageCost;}
        }
    }
    [Space]

    [Header("Direction")]
    public Vector2 flowDirection = Vector2.zero;

    internal void SetValue(Vector3 newWorldPos, int newX, int newY)
    {
        worldPosition = newWorldPos;
        gridX = newX;
        gridY = newY;
    }
}
