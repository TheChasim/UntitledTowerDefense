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
        ////boucle attraver tout les chemin
        //foreach (var way in PathFinder.pathToGoal)
        //{
        //    //trouver le spawnTile du chemin et de l'enemy soit le meme
        //    if (spawnTile.gameObject == way.spawnTile.gameObject)
        //    {
        //        //si le chemin est diferant
        //        //if (!way.pathToGoal.SequenceEqual(pathList))
        //        //{
        //            //reset les path
        //            pathList.Clear();

        //            //set le nouveaul chemin au a un TempPathlist
        //            tempPathList = way.pathToGoal;
        //        Debug.Log($"Way.pathToGoal = {way.pathToGoal.Count}");

        //            // Trouver l'indice de la tuile la plus proche.
        //            //int indexNearestTile = FindIndexOfNearestTile(tempPathList);


        //            path.Clear();
        //            // Mettre à jour le chemin avec les nouvelles tuiles depuis la tuile la plus proche.
        //            //for (int i = indexNearestTile; i >= 0; i--)
        //            //{
        //            //    pathList.Add(way.pathToGoal[i]);
        //            //}
        //            //pathList.Reverse();

        //            foreach (GameTiles tile in pathList)
        //            {
        //                path.Push(tile);
        //            }
        //        //}
        //    }
        //}



        foreach (var way in PathFinder.pathToGoal)
        {
            if(spawnTile.gameObject == way.spawnTile.gameObject)
            {
                Debug.Log($"spawn path foud and path cout : {way.pathToGoal.Count} temp paht : {way.tempPathToGoal.Count}");







            }    
        }



    }

    private int FindIndexOfNearestTile(List<GameTiles> tempPathList)
    {
        float minDistance = float.MaxValue;
        int indexNearest = 0;


        for (int i = 0; i < tempPathList.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, tempPathList[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                indexNearest = i;
            }
        }

        pos = transform.position;
        Debug.Log($"temp path list cout = {tempPathList.Count}");
        dir = tempPathList[indexNearest].transform.position;

        closeIndex = indexNearest;
        return indexNearest;
    }
}
