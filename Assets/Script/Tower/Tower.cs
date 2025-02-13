using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static HashSet<Tower> allTourel = new HashSet<Tower>();

    [SerializeField] float range;
    [SerializeField] float cooldown;
    [SerializeField] int power;
    public List<EnemyAI> enemyInRange = new List<EnemyAI>();

    [SerializeField] GameObject projectille;
    SphereCollider rangeCollider;
    GameObject target;
    bool attack = false;

    private void Awake()
    {
        allTourel.Add(this);
        rangeCollider = GetComponent<SphereCollider>();

        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         transform.position.z ); ;
    }   

    private void Update()
    {
        //set la rotation la meme que la cam
        transform.rotation = Camera.main.transform.rotation;

        if (enemyInRange.Count > 0)
        {
            OnAttack();
        }
    }

    private void OnAttack()
    {
        float dist = math.INFINITY;

        foreach (EnemyAI enemy in enemyInRange)
        {
            if (enemy != null)
            {
                if (Vector3.Distance(transform.position, enemy.gameObject.transform.position) < dist)
                {
                    target = enemy.gameObject;
                    dist = Vector3.Distance(transform.position, enemy.gameObject.transform.position);
                }
            }
        }

        if (!attack)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        attack = true;
        //Debug.Log(transform.parent.transform.position);
        Instantiate(projectille, transform.parent.transform).GetComponent<Projectile>().SetTarget(target);



        yield return new WaitForSeconds(cooldown);

        attack = false;
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
