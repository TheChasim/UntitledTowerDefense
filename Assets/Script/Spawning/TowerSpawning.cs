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
    [SerializeField] private GameObject TowerPrefab;

    internal void SpawnTower()
    {
        Transform spawnPos = GameManager.Instance.TargetTile.transform;
        ////ajoute un  0.5 pour le iso
        //spawnPos.position = new Vector3(spawnPos.position.x + 0.5f, 
        //                       spawnPos.position.y + 0.5f, 
        //                       spawnPos.position.z);
        Instantiate(TowerPrefab, spawnPos);
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
    }



}
