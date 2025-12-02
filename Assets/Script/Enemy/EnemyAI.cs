using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum EnemyType
{
    Normal,
    Heal,
    Explosif
}

public enum EnemyStatus
{
    None,
    Burn,
    poisoned,
    stun,
    wet,
    electrify,
}

public class EnemyAI : MonoBehaviour
{
    //liste static pour l'ensseble des enemie
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();

    [Header("Enemie Info")]
    [SerializeField] internal EnemyType enemyType;
    [SerializeField] float speed = 5f;
    [SerializeField] int damege = 1;
    [SerializeField] EnemyStatus enemyStatus = EnemyStatus.None;

    [Header("Healing info")]
    [SerializeField] bool isHealing = false;
    [SerializeField] float healRange = 3.5f;
    [SerializeField] float healAmount = 1.0f;
    [SerializeField] float healColddown = 0.75f;
    [SerializeField] GameObject healEffect;
    ParticleSystem healEffectParticle;
    [SerializeField] LineRenderer healLine;

    [Header("explosion info")]
    [SerializeField] float explosionRange = 3.5f;
    [SerializeField] float explosionDamage = 2f;

    [Header("Tile Info")]
    [SerializeField] GameTiles currentTile;
    [SerializeField] GameTiles nextTile;
    [SerializeField] Vector3 targetPosition;
    public bool showDirection = true;
    [Space]

    //Healt Info
    Healt healt;
    bool tileDamage = false;


    float currentSpeed
    {
        get
        {
            if (currentTile.IsSlowing)
            {
                return speed / currentTile.SlowingAmout;
            }
            else
            { return speed; }
        }

    }

    private void Awake()
    {
        //ajoute l'enemie a une liste globale
        enemyAIList.Add(this);
        //get le script pour la vie de l'enemie
        healt = GetComponent<Healt>();

        //if (canHeal)
        //{
        //    healEffect.GetComponent<ParticleSystem>().Stop();
        //}

        if (enemyType == EnemyType.Heal)
        {
            healEffectParticle = healEffect.GetComponent<ParticleSystem>();
            healEffect.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        //get le tile actuel
        SetCurrentTile();

        //set la premiere target au spawn
        if (currentTile != null)
        {
            targetPosition = currentTile.worldPosition + Vector3.up * 0.25f;
        }
    }

    private void Update()
    {
        //get le tile actuel
        //SetCurrentTile();
        //aplique l'effet de la tuille actuel
        TileEffect();
        GetStatus();

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            SetCurrentTile();
        }


        //deplace l'enemie a la prochaine tuille
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        //si l'enemie est rendue au centre de la prochaine tuille
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            //get la nouvelle tuile
            nextTile = currentTile.nextTile;

            //si il n'y a plus de nouvel tuille l'enemie est rendu a la fin
            if (nextTile == null)
            {
                Debug.Log("destroy");
                IsDead();
                if (GetComponent<Horde>())
                {
                    //si c'est une horde multiplier les dega par le nombre de pv restant
                    PlayerScript.Singleton.OnTakeDamege(damege * (int)GetComponent<Healt>().curentHealt);
                }
                else
                {
                    PlayerScript.Singleton.OnTakeDamege(damege);
                }

            }
            //set la nouvelle target a la prochaine tuile
            else
            {
                targetPosition = currentTile.nextTile.transform.position + new Vector3(0, 0.25f, 0);
            }
        }


        if (enemyType == EnemyType.Heal)
        {
            Heal();
        }

        if (enemyType == EnemyType.Explosif)
        {
            Explose();
        }


        //set l'enemie de la meme orrientation de la cam
        transform.rotation = Camera.main.transform.rotation;
        //affiche une ligne pour monttrer la prochaine tuille de l'ennemie
        Debug.DrawLine(transform.position, targetPosition, Color.blue);
    }

    private void GetStatus()
    {
        if (currentTile.IsSlowing)
        {
            enemyStatus = EnemyStatus.wet;
        }
        else if (currentTile.IsDamaging)
        {
            enemyStatus = EnemyStatus.Burn;
        }
        else
        { enemyStatus = EnemyStatus.None; }
    }

    internal void ChangeStatus(EnemyStatus status, float coldDown)
    {
        enemyStatus = status;

    }

    private void Heal()
    {
        EnemyAI target = null;
        float prevPv = 999;
        foreach (var enemy in enemyAIList)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < healRange && enemy.gameObject != this.gameObject)
            {
                if (enemy.GetComponent<Healt>().curentHealt < prevPv)
                {
                    target = enemy;
                    prevPv = target.GetComponent<Healt>().curentHealt;
                }
            }
        }

        if (target != null)
        {
            healEffect.transform.position = target.transform.position;
            healEffect.gameObject.SetActive(true);
            healEffectParticle.Play();

            healLine.SetPosition(1, target.transform.position);
            healLine.SetPosition(0, transform.position);
            healLine.enabled = true;

            if (!isHealing)
            {
                if (target.GetComponent<Healt>().curentHealt != target.GetComponent<Healt>().maxHealt)
                {
                    StartCoroutine(OnHeal(target));
                }
            }
        }
        else
        {
            healLine.enabled = false;
            healEffectParticle.Stop();
        }
    }

    IEnumerator OnHeal(EnemyAI target)
    {
        isHealing = true;
        target.OntakeHealing(healAmount);
        yield return new WaitForSeconds(healColddown);
        isHealing = false;
    }

    internal void OntakeHealing(float amount)
    {
        healt.OnTakeHealing(amount);
    }

    private void Explose()
    {

    }

    private void SetCurrentTile()
    {
        float probeRadius = 2f;
        Collider[] nearby = Physics.OverlapSphere(transform.position, probeRadius, ~0, QueryTriggerInteraction.Ignore);

        float bestDist = Mathf.Infinity;
        GameTiles nearest = null;

        foreach (var col in nearby)
        {
            GameTiles tile = col.GetComponent<GameTiles>();
            if (tile != null)
            {
                float dist = Vector3.Distance(transform.position, tile.worldPosition);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    nearest = tile;
                }
            }
        }

        if (nearest != null)
        {
            currentTile = nearest;
            return; // tuile de secours trouvée
        }
    }

    private void TileEffect()
    {
        if (currentTile == null) return;
        //si la tuille a pour effet de faire du domage start un Coroutine pour apliquer les dega
        try
        {
            if (currentTile.IsDamaging && !tileDamage)
            {
                StartCoroutine(OntileDamage(currentTile.DamageAmout));
            }
        }
        catch
        {

            Debug.Log($"currentTile= {currentTile.worldPosition}");
            Debug.Log($"currentTile.IsDamaging = {currentTile.IsDamaging}");
            Debug.Log($"tileDamage = {!tileDamage}");
        }
    }

    private IEnumerator OntileDamage(float damageAmout)
    {
        tileDamage = true;
        OnTakeDamage(currentTile.DamageAmout, currentTile.damegeType, 0.05f, 1.5f);

        yield return new WaitForSeconds(0.75f);
        tileDamage = false;
    }

    public void OnTakeDamage(float damageAmout, DamegeType type, float critChance, float critmultiplier)
    {
        float damageBonus = 0f;
        float critBonus = 1f;
        
        //here for all combo

        if(type == DamegeType.Electric && enemyStatus == EnemyStatus.wet)
        {
            damageBonus = damageAmout * 1.5f;
            critBonus = critChance * 0.15f;
        }

        //check for a crit or normal attack
        if (UnityEngine.Random.Range(0f, 1f) < critChance+critBonus)
        {
            healt.OnTakeDamage(damageAmout*critmultiplier*critBonus, true);
        }
        else
        {
            healt.OnTakeDamage(damageAmout, false);
        }

    }

    internal void IsDead()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        enemyAIList.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {

    }

}
