using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject target;
    ParticleSystem particul;
    [SerializeField] float selfDestroyTime = 3f;
    [SerializeField] float speed;
    [SerializeField] internal float damage;
    [SerializeField] internal DamegeType DamegeType;
    [SerializeField] internal float critChance;
    [SerializeField] internal float critMultiplier;

    private void Awake()
    {
        particul = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    internal void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            //other.GetComponent<Healt>().OnTakeDamage(damage);
            other.GetComponent<EnemyAI>().OnTakeDamage(damage, DamegeType,critChance,critMultiplier);

            StartCoroutine(SelfDestroy());
        }
    }

    private IEnumerator SelfDestroy()
    {
        particul.Stop();

        yield return new WaitForSeconds(selfDestroyTime);

        Destroy(gameObject);
    }
}
