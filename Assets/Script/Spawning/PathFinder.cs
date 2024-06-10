using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PathToGoal
{
    internal GameTiles spawnTile;
    internal List<GameTiles> pathToGoal = new List<GameTiles>();
}



public class PathFinder : MonoBehaviour
{
    GameTiles[,] gameTiles;
    internal List<GameTiles> spawnTile;
    internal GameTiles endTile;
    static internal List<PathToGoal> pathToGoal = new List<PathToGoal>();
    PathToGoal currentPath = new PathToGoal();

    int ColCount = 0;
    int RowCount = 0;

    //constructeur
    public PathFinder(GameTiles[,] NewGametiles, List<GameTiles> NewSpawnTile, GameTiles NewEndTil, int NewCol, int NewRow)
    {
        gameTiles = NewGametiles;
        spawnTile = NewSpawnTile;
        endTile = NewEndTil;
        ColCount = NewCol;
        RowCount = NewRow;

    }
    internal void SetValue(GameTiles[,] NewGametiles, List<GameTiles> NewSpawnTile, GameTiles NewEndTil, int NewCol, int NewRow)
    {
        gameTiles = NewGametiles;
        endTile = NewEndTil;
        ColCount = NewCol;
        RowCount = NewRow;

        foreach(var spawn in NewSpawnTile)
        {
            PathToGoal tempPath = new PathToGoal();
            tempPath.spawnTile = spawn;
            pathToGoal.Add(tempPath);
        }
    }
    public PathFinder()
    { }


    internal void SetPath()
    {
        foreach (var t in gameTiles)
        {
            t.SetPathColor(false);
        }
       

        foreach (var spawn in pathToGoal)
        {

            var path = PathFinding(spawn.spawnTile, endTile);
            var tile = endTile;

            while (tile != null )
            {
                //Debug.Log(path);
                //Debug.Log(tile);

                spawn.pathToGoal.Add(tile);
                tile.SetPathColor(true);
                tile = path[tile];

                //Debug.Log(tile.ToString());

            }

            //currentPath.spawnTile = spawn;
            //pathToGoal.Add(currentPath);

            Debug.Log("Path Created");
        }

        //play = true;
        //spwaning.Spawning(spawnTile, pathToGoal);
    }
    private Dictionary<GameTiles, GameTiles> PathFinding(GameTiles sourceTile, GameTiles targetTile)
    {
        Dictionary<GameTiles, GameTiles> path = PathFindingAlgo(sourceTile, targetTile, avoidDamage: true, avoidSlow: true);

        // Si impossible de trouver un chemin évitant les tuiles dangereuses, essayer d'éviter les tuiles lentes
        if (path == null)
        {
            path = PathFindingAlgo(sourceTile, targetTile, avoidDamage: true, avoidSlow: false);
        }

        // Si toujours impossible, trouver le chemin le plus court en ignorant les conditions
        if (path == null)
        {
            path = PathFindingAlgo(sourceTile, targetTile, avoidDamage: false, avoidSlow: false);
        }

        return path;
    }
    private Dictionary<GameTiles, GameTiles> PathFindingAlgo(GameTiles sourceTile, GameTiles targetTile, bool avoidDamage, bool avoidSlow)
    {
        var dist = new Dictionary<GameTiles, int>();
        var prev = new Dictionary<GameTiles, GameTiles>();
        var Q = new List<GameTiles>();

        foreach (var v in gameTiles)
        {
            dist[v] = int.MaxValue;
            prev[v] = null;
            Q.Add(v);
        }

        dist[sourceTile] = 0;

        while (Q.Count > 0)
        {
            GameTiles u = null;
            int minDistance = int.MaxValue;

            foreach (var v in Q)
            {
                if (dist[v] < minDistance)
                {
                    minDistance = dist[v];
                    u = v;
                }
            }

            if (u == null || u == targetTile)
            {
                break;
            }

            Q.Remove(u);

            foreach (var v in FindNeighbor(u))
            {
                if (!Q.Contains(v) || v.IsBloced || (avoidDamage && v.IsDamaging) || (avoidSlow && v.IsSlowing))
                {
                    continue;
                }

                int alt = dist[u] + 1; // Vous pouvez ajuster cette valeur pour les tuiles lentes ou dangereuses

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        if (prev[targetTile] == null)
        {
            return null; // Aucun chemin trouvé
        }

        return prev;
    }

    private List<GameTiles> FindNeighbor(GameTiles u)
    {
        var result = new List<GameTiles>();

        //if (u.X - 1 >= 0)
        //{ result.Add(gameTiles[u.X - 1, u.Y]); }
        //if (u.X + 1 < ColCount)
        //{ result.Add(gameTiles[u.X + 1, u.Y]); }
        //if (u.Y - 1 >= 0)
        //{ result.Add(gameTiles[u.X, u.Y - 1]); }
        //if (u.Y + 1 < RowCount)
        //{ result.Add(gameTiles[u.X, u.Y + 1]); }

        if (u.X - 1 >= 0)
        { result.Add(gameTiles[u.Y, u.X - 1]); }
        if (u.X + 1 < ColCount)
        { result.Add(gameTiles[u.Y, u.X + 1]); }
        if (u.Y - 1 >= 0)
        { result.Add(gameTiles[u.Y - 1, u.X]); }
        if (u.Y + 1 < RowCount)
        { result.Add(gameTiles[u.Y + 1, u.X]); }

        return result;
    }

    internal void Reset()
    {
        spawnTile.Clear();
    }
}
