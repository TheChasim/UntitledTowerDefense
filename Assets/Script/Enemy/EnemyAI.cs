using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyAI : MonoBehaviour
{
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();
    private Stack<GameTiles> path = new Stack<GameTiles>();
    public List<GameTiles> pathList = new List<GameTiles>();
    public List<GameTiles> tempPathList = new List<GameTiles>();
    internal GameTiles spawnTile;
    public GameTiles currentTile;

    public int closeIndex;
    public bool showDirection = true;

    Healt healt;
    bool tileDamage = false;

    [SerializeField] float speed = 5f;

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
    }

    private void Update()
    {
        SetCurrentTile();

        TileEffect();

        if (path.Count != 0)
        {
            if (showDirection)
            {
                Debug.DrawLine(transform.position, path.Peek().transform.position, Color.blue);
            }

            Vector3 desPos = path.Peek().transform.position;

            desPos.y += 0.25f;

            transform.position = Vector3.MoveTowards(transform.position, desPos, currentSpeed * Time.deltaTime);


            if (Vector3.Distance(transform.position, desPos) < 0.1f)
            {
                path.Pop();
            }
        }
        else
        {
            Destroy(gameObject);
        }


        //set la rotation la meme que la cam
        transform.rotation = Camera.main.transform.rotation;

    }

    private void SetCurrentTile()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);

            if (hit.collider.GetComponent<GameTiles>())
            {
                currentTile = hit.collider.GetComponent<GameTiles>();
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

    internal void SetPath()
    {
        path.Clear();
        pathList.Clear();

        pathList = FindAnyObjectByType<NewPathFinder>().GetComponent<NewPathFinder>().FindPathAStar(currentTile);
        pathList.Reverse();

        foreach (var tile in pathList)
        {
            path.Push(tile);
        }
    }

    internal void SetPath(GameTiles spawnTile)
    {
        //this.spawnTile = spawnTile;
        //path.Clear();
        //pathList.Clear();

        //foreach (var way in PathFinder.pathToGoal)
        //{
        //    if (spawnTile.gameObject == way.spawnTile.gameObject)
        //    {
        //        pathList = way.pathToGoal;

        //        foreach (GameTiles tile in way.pathToGoal)
        //        {
        //            path.Push(tile);
        //        }

        //    }

        //}
        path.Clear();
        pathList.Clear();

        pathList = FindAnyObjectByType<NewPathFinder>().GetComponent<NewPathFinder>().FindPathAStar(spawnTile);
        pathList.Reverse();

        foreach (var tile in pathList)
        {
            path.Push(tile);
        }
    }

    internal void setNewPath()
    {
        //boucle attraver tout les chemin possible
        foreach (var way in PathFinder.pathToGoal)
        {
            //si l'ennemi et le path on le meme spawn commencer le check
            if (spawnTile.gameObject == way.spawnTile.gameObject)
            {
                //set le pathlist avec le nouveau chemien
                pathList = way.pathToGoal;
                //cherche l'index le tuile du nouveau chemin avec sa posiont actuelle 
                closeIndex = FindIndexOfNearestTile();

                //efface l'acien chemin temporaire
                tempPathList.Clear();

                //cree le nouveau chemin temp appartir de l'index 
                for (int i = closeIndex; i >= 0; i--)
                {
                    tempPathList.Add(way.pathToGoal[i]);
                }

                tempPathList.Reverse();

                //supprime l'ancien path pour mettre le nouveau
                path.Clear();
                foreach (GameTiles tile in tempPathList)
                {
                    path.Push(tile);
                }

                //acossi le chemin temporaire avec le vrai
                pathList = tempPathList;


            }
        }
    }

    private int FindIndexOfNearestTile()
    {
        float minDistance = float.MaxValue;
        int indexNearest = 0;

        //boucle in all tile in the path to find the closest
        for (int i = 0; i < pathList.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, pathList[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                indexNearest = i;
            }
        }
        return indexNearest;
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
