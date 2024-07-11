using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();
    private Stack<GameTiles> path = new Stack<GameTiles>();
    public List<GameTiles> pathList = new List<GameTiles>();
    public List<GameTiles> tempPathList = new List<GameTiles>();
    internal GameTiles spawnTile;

    public int closeIndex;
    public bool showDirection = true;
    public Vector3 pos;
    public Vector3 dir;

    [SerializeField] float speed = 5f;

    private void Awake()
    {
        enemyAIList.Add(this);
    }

    private void Update()
    {
        if (path.Count != 0)
        {
            if (showDirection)
            {
                Debug.DrawLine(transform.position, path.Peek().transform.position, Color.blue);
            }

            Vector3 desPos = path.Peek().transform.position;

            transform.position = Vector3.MoveTowards(transform.position, desPos, speed * Time.deltaTime);


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

    internal void SetPath(GameTiles spawnTile)
    {
        this.spawnTile = spawnTile;
        path.Clear();
        pathList.Clear();

        foreach (var way in PathFinder.pathToGoal)
        {
            if (spawnTile.gameObject == way.spawnTile.gameObject)
            {
                pathList = way.pathToGoal;

                foreach (GameTiles tile in way.pathToGoal)
                {
                    path.Push(tile);
                }

            }

        }
    }

    private void OnDestroy()
    {
        enemyAIList.Remove(this);
    }

    internal void IsDead()
    {
        Destroy(gameObject);
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
}
