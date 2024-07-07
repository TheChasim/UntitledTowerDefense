using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static HashSet<Tower> allTourel = new HashSet<Tower>();

    [SerializeField] float range;
    [SerializeField] float cooldown;
    [SerializeField] int power;
    public List<EnemyAI> enemyInRange = new List<EnemyAI>();

    SphereCollider rangeCollider;


    private void Awake()
    {
        allTourel.Add(this);
        rangeCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        //set la rotation la meme que la cam
        transform.rotation = Camera.main.transform.rotation;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyAI>())
        {
            //Debug.Log($"add {other.name} au range de {gameObject.name}");
            enemyInRange.Add(other.gameObject.GetComponent<EnemyAI>());
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyAI>())
        {
            //Debug.Log($"Remove {other.name} au range de {gameObject.name}");
            enemyInRange.Remove(other.gameObject.GetComponent<EnemyAI>());
        }
    }



    internal void OnRevome()
    {
        allTourel.Remove(this);
        Destroy(gameObject);
    }

}
