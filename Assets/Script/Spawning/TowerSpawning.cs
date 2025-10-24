using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

public class TowerSpawning : MonoBehaviour
{
    //const
    const int MaxTower = 5;

    //singleton
    public static TowerSpawning Instance;


    [SerializeField] internal List<GameObject> allTower = new List<GameObject>();
    [SerializeField] internal List<GameObject> towers = new List<GameObject>();
    GameObject selectTower;

    internal void OnSelectIndex(int indextower)
    {
        selectTower = towers[indextower];
    }

    internal void SpawnTower()
    {
        Transform spawnPos = GameManager.Instance.TargetTile.transform;
        Instantiate(selectTower, spawnPos);
    }

    private void Awake()
    {
        //Creation of singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (selectTower == null)
        {
            selectTower = towers.First();
        }
    }



}
