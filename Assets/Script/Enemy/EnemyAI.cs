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

public class EnemyAI : MonoBehaviour
{
    //liste static pour l'ensseble des enemie
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();

    [Header("Enemie Info")]
    [SerializeField] internal EnemyType enemyType;
    [SerializeField] float speed = 5f;

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
        SetCurrentTile();
        //aplique l'effet de la tuille actuel
        TileEffect();

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
        //si le ray touche une tuille
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            //affiche le ray
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);

            //si le ray touche une tuile
            if (hit.collider.GetComponent<GameTiles>())
            {
                //set la tuille du ray a currentTile
                currentTile = hit.collider.GetComponent<GameTiles>();
            }
        }
    }

    private void TileEffect()
    {
        //si la tuille a pour effet de faire du domage start un Coroutine pour apliquer les dega
        if (currentTile.IsDamaging && !tileDamage)
        {
            StartCoroutine(OntileDamage(currentTile.DamageAmout));
        }
    }

    private IEnumerator OntileDamage(float damageAmout)
    {
        tileDamage = true;
        OnTakeDamage(currentTile.DamageAmout);

        yield return new WaitForSeconds(1);
        tileDamage = false;
    }

    public void OnTakeDamage(float damageAmout)
    {
        healt.OnTakeDamage(damageAmout);
    }

    internal void IsDead()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        enemyAIList.Remove(this);
    }

}
