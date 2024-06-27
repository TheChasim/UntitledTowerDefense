using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static HashSet<Tower> allTourel = new HashSet<Tower>();

    private void Awake()
    {
        allTourel.Add(this);
    }

    internal void OnRevome()
    {
        allTourel.Remove(this);
        Destroy(gameObject);
    }

}
