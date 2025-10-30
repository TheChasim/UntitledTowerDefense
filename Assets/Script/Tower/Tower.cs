using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

enum TowerType
{
    Projectil,
    AOE,
    Camp,
}

[System.Flags]
public enum TypeOfEffect
{
    None = 0,
    OverTime = 1,  // 1
    Burst = 2,  // 2
    CanMove = 4,  // 4
    Spawning = 8,  // 8
}
public class Tower : MonoBehaviour
{
    public static HashSet<Tower> allTourel = new HashSet<Tower>();

    [Header("Tower Info")]
    [SerializeField] TowerType type;
    [SerializeField] TypeOfEffect effect;
    [SerializeField] float range;
    [SerializeField] float minRange;
    [SerializeField] float cooldown;
    [SerializeField] float power;
    [SerializeField] float rotationSpeed = 5;
    [SerializeField] int maxUnit = 7;
    internal HashSet<GameObject> unitlist = new HashSet<GameObject>();
    static internal HashSet<GameObject> allUnitList = new HashSet<GameObject>();
    public List<EnemyAI> enemyInRange = new List<EnemyAI>();
    [SerializeField] GameObject projectille;
    [SerializeField] GameObject damagingZone;
    [SerializeField] GameObject spwaningUnit;
    [Space]

    SphereCollider rangeCollider;
    GameObject target;
    internal List<EnemyAI> targets = new List<EnemyAI>();
    bool attack = false;

    private void Awake()
    {
        allTourel.Add(this);
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.radius = range;

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
        if (type == TowerType.Projectil)
        {
            if (enemyInRange.Count > 0)
            {
                if (type.HasFlag(TowerType.Projectil))
                {
                    OnAttackProjectil();
                }
            }
        }
        else if (type == TowerType.AOE)
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
                            if (effect.HasFlag(TypeOfEffect.OverTime))
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
                            if (effect.HasFlag(TypeOfEffect.OverTime))
                            {
                                //damagingZone.GetComponent<ParticleSystem>().Play();
                                PlayEffect();
                            }
                        }
                    }
                }
            }
            else if (effect.HasFlag(TypeOfEffect.OverTime))
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
        else if (type == TowerType.Camp)
        {
            if (!attack && unitlist.Count < maxUnit) //utilise attack pour le coldown entre chaque spawn d<uniter afin d'eviter trop de variable
            {
                StartCoroutine(OnSpawnUnit());
            }

            if (enemyInRange.Count > 0)
            {
                SendUnit();
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
            //Debug.Log($"{name} stop particul effect");
        }
        else if (damagingZone.GetComponent<VisualEffect>())
        {
            damagingZone.GetComponent<VisualEffect>().Stop();
            //Debug.Log($"{name} stop Vfx");
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

                    if (effect.HasFlag(TypeOfEffect.CanMove))
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
        if (type.HasFlag(TowerType.Projectil))
        {
            Instantiate(projectille, transform.parent.transform).GetComponent<Projectile>().SetTarget(target);
        }

        yield return new WaitForSeconds(cooldown);

        attack = false;
    }

    private void SendUnit()
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


                    Debug.DrawLine(transform.position, target.transform.position, Color.red);

                    foreach (var unit in unitlist)
                    {
                        if (unit != null)
                        {
                            unit.GetComponent<Unit>().target = target;
                        }
                    }

                }
            }
        }
    }

    private IEnumerator OnSpawnUnit()
    {
        attack = true;

        GameObject unit = Instantiate(spwaningUnit, transform.parent.transform);
        unit.GetComponent<Unit>().campRef = this;
        unitlist.Add(unit);

        yield return new WaitForSeconds(cooldown);

        attack = false;
    }

    private IEnumerator AttackZone()
    {
        attack = true;

        if (effect.HasFlag(TypeOfEffect.Burst))
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
        if (effect.HasFlag(TypeOfEffect.Spawning))
        {
            foreach (var unit in unitlist)
            {
                Destroy(unit.gameObject);
            }
        }

        allTourel.Remove(this);
        Destroy(gameObject);
    }

}
