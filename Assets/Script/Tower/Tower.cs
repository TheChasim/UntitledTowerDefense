using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public class Tower : MonoBehaviour
{
    public static HashSet<Tower> allTourel = new HashSet<Tower>();

    [Header("Tower Info")]
    [SerializeField] float range;
    [SerializeField] float minRange;
    [SerializeField] float cooldown;
    [SerializeField] float power;
    [SerializeField] float rotationSpeed = 5;
    public List<EnemyAI> enemyInRange = new List<EnemyAI>();
    //[SerializeField] Collider rangeCollider;
    [SerializeField] GameObject projectille;
    [SerializeField] GameObject damagingZone;
    [Space]

    [Header("Type of attack")]
    [SerializeField] bool projectil;
    [SerializeField] bool AOE;
    [SerializeField] bool canMove;

    [Header("Type of effect")]
    [SerializeField] bool overTime;
    [SerializeField] bool brust;

    SphereCollider rangeCollider;
    GameObject target;
    internal List<EnemyAI> targets = new List<EnemyAI>();
    bool attack = false;

    private void Awake()
    {
        allTourel.Add(this);
        rangeCollider = GetComponent<SphereCollider>();

        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         transform.position.z);
        if (damagingZone != null)
        {
            StopEffect();
        }


    }

    private void Update()
    {
        //set la rotation la meme que la cam
        transform.rotation = Camera.main.transform.rotation;

        if (projectil)
        {
            if (enemyInRange.Count > 0)
            {
                if (projectil)
                {
                    OnAttackProjectil();
                }
            }
        }
        else if (AOE)
        {
            if (enemyInRange.Count > 0)
            {
                if (damagingZone != null)
                {
                    OnAttackZone();

                    //si c'est une particul effect
                    if (damagingZone.GetComponent<ParticleSystem>())
                    {
                        if (!damagingZone.GetComponent<ParticleSystem>().isPlaying)
                        {
                            if (overTime)
                            {
                                //damagingZone.GetComponent<ParticleSystem>().Play();
                                PlayEffect();
                            }
                        }
                    }
                    else if (damagingZone.GetComponent<VisualEffect>())
                    {
                        if (!damagingZone.GetComponent<VisualEffect>().HasAnySystemAwake())
                        {
                            if (overTime)
                            {
                                //damagingZone.GetComponent<ParticleSystem>().Play();
                                PlayEffect();
                            }
                        }
                    }
                }
            }
            else if (overTime)
            {
                if (damagingZone.GetComponent<ParticleSystem>())
                {
                    if (damagingZone.GetComponent<ParticleSystem>().isPlaying)
                    {
                        StopEffect();
                    }
                }
                else if (damagingZone.GetComponent<VisualEffect>())
                {
                    if (damagingZone.GetComponent<VisualEffect>().HasAnySystemAwake())
                    {
                        StopEffect();
                    }
                }
            }
        }

        //clean la liste
        try
        {
            foreach (EnemyAI target in targets)
            {
                if (target == null)
                { targets.Remove(target); }
            }
            foreach (EnemyAI target in enemyInRange)
            {
                if (target == null)
                { enemyInRange.Remove(target); }
            }
        }
        catch (Exception ex)
        {

        }
    }

    private void PlayEffect()
    {
        if (damagingZone.GetComponent<ParticleSystem>())
        {
            damagingZone.GetComponent<ParticleSystem>().Play();
            Debug.Log($"{name} play particul effect");
        }
        else if (damagingZone.GetComponent<VisualEffect>())
        {
            damagingZone.GetComponent<VisualEffect>().Play();
            Debug.Log($"{name} play Vfx");
        }
    }
    private void StopEffect()
    {
        if (damagingZone.GetComponent<ParticleSystem>())
        {
            damagingZone.GetComponent<ParticleSystem>().Stop();
            Debug.Log($"{name} stop particul effect");
        }
        else if (damagingZone.GetComponent<VisualEffect>())
        {
            damagingZone.GetComponent<VisualEffect>().Stop();
            Debug.Log($"{name} stop Vfx");
        }
    }


    private void OnAttackZone()
    {
        float dist = math.INFINITY;

        foreach (EnemyAI enemy in enemyInRange)
        {
            if (enemy != null)
            {
                if (Vector3.Distance(transform.position, enemy.gameObject.transform.position) < dist)
                {
                    target = enemy.gameObject;

                    if (canMove)
                    {
                        dist = Vector3.Distance(transform.position, enemy.gameObject.transform.position);
                        Vector3 direction = target.transform.position - damagingZone.transform.position;
                        direction = new Vector3(direction.x, direction.y - 0.5f, direction.z);
                        Quaternion targetRotation = Quaternion.LookRotation(direction); // Rotation cible basée sur la direction

                        // Rotation progressive vers la cible
                        damagingZone.transform.rotation = Quaternion.RotateTowards(
                            damagingZone.transform.rotation,
                            targetRotation,
                            rotationSpeed * Time.deltaTime);
                    }
                }
            }
        }

        if (!attack)
        {
            StartCoroutine(AttackZone());
        }
    }

    private void OnAttackProjectil()
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
        if (projectil)
        {
            Instantiate(projectille, transform.parent.transform).GetComponent<Projectile>().SetTarget(target);
        }

        yield return new WaitForSeconds(cooldown);

        attack = false;
    }

    private IEnumerator AttackZone()
    {
        attack = true;

        if (brust)
        {
            damagingZone.GetComponent<ParticleSystem>().Play();
        }

        foreach (var enemie in targets)
        {
            if (enemie != null)
            {
                enemie.GetComponent<Healt>().OnTakeDamage(power);

            }
        }

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

        if (damagingZone != null)
        {
            if (other == damagingZone.GetComponent<CapsuleCollider>())
            {
                if (other.gameObject.GetComponent<EnemyAI>())
                {
                    //Debug.Log($"add {other.name} au range de {gameObject.name}");
                    targets.Add(other.gameObject.GetComponent<EnemyAI>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.GetComponent<EnemyAI>())
        {
            //Debug.Log($"add {other.name} au range de {gameObject.name}");
            enemyInRange.Remove(other.gameObject.GetComponent<EnemyAI>());
        }

        if (damagingZone != null)
        {
            if (other == damagingZone.GetComponent<CapsuleCollider>())
            {
                if (other.gameObject.GetComponent<EnemyAI>())
                {
                    //Debug.Log($"add {other.name} au range de {gameObject.name}");
                    targets.Remove(other.gameObject.GetComponent<EnemyAI>());
                }
            }
        }

    }

    internal void OnRevome()
    {
        allTourel.Remove(this);
        Destroy(gameObject);
    }

}
