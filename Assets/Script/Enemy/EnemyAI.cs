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
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();
    private Stack<GameTiles> path = new Stack<GameTiles>();
    public List<GameTiles> pathList = new List<GameTiles>();
    public List<GameTiles> tempPathList = new List<GameTiles>();
    internal GameTiles spawnTile;
    public GameTiles currentTile;
    NewPathFinder pathFinder;

    public int closeIndex;
    public bool showDirection = true;

    Healt healt;
    bool tileDamage = false;

    [SerializeField] float speed = 5f;

    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] GameTiles nextTile;

    private float _currentSpeed;
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
        enemyAIList.Add(this);

        healt = GetComponent<Healt>();
        //pathFinder = FindAnyObjectByType<NewPathFinder>().GetComponent<NewPathFinder>();
    }

    private void Start()
    {
        SetCurrentTile();
        if (currentTile != null)
        {
            targetPosition = currentTile.worldPosition + Vector3.up * 0.5f;
        }
    }

    private void Update()
    {


        TileEffect();
        SetCurrentTile();

        //if (path.Count != 0)
        //{
        //    if (showDirection)
        //    {
        //        Debug.DrawLine(transform.position, path.Peek().transform.position, Color.blue);
        //    }

        //    Vector3 desPos = path.Peek().transform.position;

        //    desPos.y += 0.25f;

        //    transform.position = Vector3.MoveTowards(transform.position, desPos, currentSpeed * Time.deltaTime);


        //    if (Vector3.Distance(transform.position, desPos) < 0.1f)
        //    {
        //        path.Pop();
        //    }
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

        //transform.position = Vector3.MoveTowards(transform.position, currentTile.flowDirection, currentSpeed * Time.deltaTime);

        // Déplacement basé sur la direction du Flow Field

        //moveDirection = new Vector3(currentTile.flowDirection.x, 0, currentTile.flowDirection.z);


        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            nextTile = GetNextTile();

            if (nextTile == null)
            {
                Debug.Log("destroy");
                Destroy(gameObject);

            }
            else
            {
                targetPosition = currentTile.nextTile.transform.position + new Vector3(0, 0.25f, 0);
            }

        }
        //transform.position += moveDirection * currentSpeed * Time.deltaTime;


        //set la rotation la meme que la cam
        transform.rotation = Camera.main.transform.rotation;
        Debug.DrawLine(transform.position, targetPosition, Color.blue);

    }

    private GameTiles GetNextTile()
    {
        return currentTile.nextTile;
    }

    private void SetCurrentTile()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);

            if (hit.collider.GetComponent<GameTiles>())
            {

                currentTile = hit.collider.GetComponent<GameTiles>();
                moveDirection = new Vector3(currentTile.flowDirection.x, 0, currentTile.flowDirection.z);
                //if (targetPosition == Vector3.zero)
                //{
                //    targetPosition = currentTile.transform.position;
                //}
                //targetPosition = currentTile.nextTile.transform.position;

            }
        }
    }

    private void TileEffect()
    {
        //currentTile = path.Peek();

        if (currentTile.IsDamaging && !tileDamage)
        {
            StartCoroutine(OntileDamage(currentTile.DamageAmout));
        }
    }

    //private float lastPathUpdateTime = 0f;
    //private float minPathUpdateInterval = 1f; // Minimum 1 secondes entre chaque recalcul

    //async internal Task SetPath()
    //{
    //    // Vérifier si le chemin est bloqué
    //    if (PathIsBlock() && Time.time - lastPathUpdateTime > minPathUpdateInterval)
    //    {
    //        lastPathUpdateTime = Time.time; // Mise à jour du dernier recalcul

    //        Debug.Log($"Recalcul du chemin pour {gameObject.name}");

    //        // Sauvegarde temporaire
    //        List<GameTiles> oldPath = new List<GameTiles>(pathList);

    //        // Attendre le recalcul du chemin
    //        List<GameTiles> newPath = await pathFinder.CalculatePathsMultithreaded(currentTile);

    //        if (newPath != null && newPath.Count > 0)
    //        {
    //            path.Clear();
    //            newPath.Reverse();
    //            pathList = newPath;
    //            foreach (var tile in newPath)
    //            {
    //                path.Push(tile);
    //            }
    //            Debug.Log($"Nouveau chemin assigné à {gameObject.name}");
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Impossible de recalculer le chemin, réutilisation de l'ancien chemin.");
    //            pathList = oldPath;
    //        }
    //    }
    //}

    //private bool PathIsBlock()
    //{
    //    foreach (var tile in pathList)
    //    {
    //        if (tile.IsBloced)
    //        {
    //            Debug.Log("path is block");
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    //internal void SetPath(GameTiles spawnTile)
    //{
    //    path.Clear();
    //    pathList.Clear();

    //    pathList = FindAnyObjectByType<NewPathFinder>().GetComponent<NewPathFinder>().FindPathAStar(spawnTile);
    //    pathList.Reverse();

    //    foreach (var tile in pathList)
    //    {
    //        path.Push(tile);
    //    }
    //}

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
