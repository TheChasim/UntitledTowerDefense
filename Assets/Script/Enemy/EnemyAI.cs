using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Burst.CompilerServices;
using System;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour
{
    //liste static pour l'ensseble des enemie
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();

    [Header("Tile Info")]
    [SerializeField] GameTiles currentTile;
    [SerializeField] GameTiles nextTile;
    [SerializeField] Vector3 targetPosition;
    public bool showDirection = true;
    [Space]

    //Healt Info
    Healt healt;
    bool tileDamage = false;

    //Speed info
    [Header("Speed")]
    [SerializeField] float speed = 5f;
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

        //set l'enemie de la meme orrientation de la cam
        transform.rotation = Camera.main.transform.rotation;
        //affiche une ligne pour monttrer la prochaine tuille de l'ennemie
        Debug.DrawLine(transform.position, targetPosition, Color.blue);
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

    private void OnTakeDamage(float damageAmout)
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
