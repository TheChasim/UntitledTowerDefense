using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder
{
    GameTiles[,] gameTiles;
    internal GameTiles spawnTile;
    internal GameTiles endTile;
    List<GameTiles> pathToGoal = new List<GameTiles>();

    int ColCount = 0;
    int RowCount = 0;

    //constructeur
    public PathFinder(GameTiles[,] NewGametiles, GameTiles NewSpawnTile, GameTiles NewEndTil, int NewCol, int NewRow)
    {
        gameTiles = NewGametiles;
        spawnTile = NewSpawnTile;
        endTile = NewEndTil;
        ColCount = NewCol;
        RowCount = NewRow;

    }


    internal void SetPath()
    {
        foreach (var t in gameTiles)
        {
            t.SetPathColor(false);
        }

        var path = PathFinding(spawnTile, endTile);
        var tile = endTile;

        while (tile != null)
        {
            pathToGoal.Add(tile);
            tile.SetPathColor(true);
            tile = path[tile];
        }

        Debug.Log("Path Created");

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

}
