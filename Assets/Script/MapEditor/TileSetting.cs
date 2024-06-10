using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetting : MonoBehaviour
{
    [SerializeField] List<NonInteractableInEditor> tiles = new List<NonInteractableInEditor>();


    //internal void OnChangeEditable()
    //{
    //    foreach(var tile in tiles)
    //    {
    //        tile.OnChangeEditable();
    //    }
    //}

    //internal void OnchangeHide()
    //{
    //    foreach (var tile in tiles)
    //    {
    //        tile.OnChangeHide();
    //    }
    //}

    internal void OnReset()
    {
        foreach( var tile in tiles)
        {
            tile.OnReset();
        }
    }
}
