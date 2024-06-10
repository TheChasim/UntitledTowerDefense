using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();
    private Stack<GameTiles> path = new Stack<GameTiles>();
    //public List<GameTiles> pathList = new List<GameTiles>();

    public bool showDirection = true;

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
    }

    internal void SetPath(GameTiles spawnTile)
    {
        path.Clear();

        foreach (var way in PathFinder.pathToGoal)
        {
            if (spawnTile.gameObject == way.spawnTile.gameObject)
            {
                foreach (GameTiles tile in way.pathToGoal)
                {
                    path.Push(tile);
                }

            }

        }


        //pathList = pathToGoal;
    }

    private void OnDestroy()
    {
        enemyAIList.Remove(this);
    }


}
