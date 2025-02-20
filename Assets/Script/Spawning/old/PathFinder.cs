using System.Collections.Generic;
using UnityEngine;

public class PathToGoal
{
    internal GameTiles spawnTile;
    internal List<GameTiles> pathToGoal = new List<GameTiles>();
    internal List<GameTiles> tempPathToGoal = new List<GameTiles>();
    //internal int leght;
}

public class PathFinder : MonoBehaviour
{
    GameTiles[,] gameTiles;
    internal List<GameTiles> spawnTile;
    internal GameTiles endTile;
    static internal List<PathToGoal> pathToGoal = new List<PathToGoal>();
    //PathToGoal currentPath = new PathToGoal();

    int ColCount = 0;
    int RowCount = 0;

    internal void SetValue(GameTiles[,] NewGametiles, List<GameTiles> NewSpawnTile, GameTiles NewEndTil, int NewCol, int NewRow)
    {
        gameTiles = NewGametiles;
        endTile = NewEndTil;
        ColCount = NewCol;
        RowCount = NewRow;

        foreach (var spawn in NewSpawnTile)
        {
            PathToGoal tempPath = new PathToGoal();
            tempPath.spawnTile = spawn;
            pathToGoal.Add(tempPath);
        }
    }


    internal void SetPath()
    {
        foreach (var t in gameTiles)
        {
            t.SetPathColor(false);
        }


        foreach (var spawn in pathToGoal)
        {
            Debug.Log($"Spwan : {spawn.spawnTile.name} | End : {endTile}");

            spawn.pathToGoal.Clear();
            var path = PathFinding(spawn.spawnTile, endTile);
            var tile = endTile;

            Debug.Log($"base path = {path}");

            while (tile != null)
            {
                spawn.pathToGoal.Add(tile);
                tile.SetPathColor(true);
                Debug.Log(tile.name);
                tile = path[tile];
            }

            Debug.Log($"Path Created and count : {spawn.pathToGoal.Count}");
        }
    }

    internal List<GameTiles> EnemySetPath(GameTiles pos, GameTiles end)
    {
        foreach (var t in gameTiles)
        {
            t.SetPathColor(false);
        }

        List<GameTiles> newPath = new List<GameTiles>();
        var path = PathFinding(pos, end);
        var tile = end;

        Debug.Log($"base path = {path}");

        while (tile != null)
        {
            newPath.Add(tile);
            tile.SetPathColor(true);
            tile = path[tile];
        }

        //Debug.Log($"Path Created and count : {spawn.pathToGoal.Count}");

        return newPath;
    }


    internal List<int> SetTempPath()
    {
        List<int> leght = new List<int>();

        foreach (var spawn in pathToGoal)
        {
            spawn.tempPathToGoal.Clear();
            var path = PathFinding(spawn.spawnTile, endTile);
            var tile = endTile;

            if (path != null)
            {
                while (tile != null)
                {
                    spawn.tempPathToGoal.Add(tile);
                    //tile.SetPathColor(true);

                    //Debug.Log(tile);
                    //Debug.Log(path.Count);

                    tile = path[tile];
                }


                leght.Add(spawn.tempPathToGoal.Count);
            }
            else
            {
                leght.Add(0);
            }

            Debug.Log("Temp Path Created");
        }

        return leght;
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

//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class PriorityQueue<T>
//{
//    private List<(T Element, int Priority)> elements = new List<(T, int)>();

//    public int Count => elements.Count;

//    public void Enqueue(T element, int priority)
//    {
//        elements.Add((element, priority));
//        elements.Sort((x, y) => x.Priority.CompareTo(y.Priority)); // Trie par priorité croissante
//    }

//    public T Dequeue()
//    {
//        if (elements.Count == 0)
//            throw new InvalidOperationException("The priority queue is empty.");

//        var item = elements[0];
//        elements.RemoveAt(0); // Supprime l'élément avec la plus haute priorité (le premier)
//        return item.Element;
//    }

//    public bool Contains(T element)
//    {
//        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.Element, element));
//    }
//}

//public class PathToGoal
//{
//    internal GameTiles spawnTile;
//    internal List<GameTiles> pathToGoal = new List<GameTiles>();
//    internal List<GameTiles> tempPathToGoal = new List<GameTiles>();
//}

//public class PathFinder : MonoBehaviour
//{
//    GameTiles[,] gameTiles;
//    internal List<GameTiles> spawnTile;
//    internal GameTiles endTile;
//    static internal List<PathToGoal> pathToGoal = new List<PathToGoal>();
//    Dictionary<(GameTiles, GameTiles), List<GameTiles>> cachedPaths = new Dictionary<(GameTiles, GameTiles), List<GameTiles>>();

//    int ColCount = 0;
//    int RowCount = 0;

//    internal void SetValue(GameTiles[,] NewGametiles, List<GameTiles> NewSpawnTile, GameTiles NewEndTil, int NewCol, int NewRow)
//    {
//        gameTiles = NewGametiles;
//        endTile = NewEndTil;
//        ColCount = NewCol;
//        RowCount = NewRow;

//        foreach (var spawn in NewSpawnTile)
//        {
//            PathToGoal tempPath = new PathToGoal();
//            tempPath.spawnTile = spawn;
//            pathToGoal.Add(tempPath);
//        }
//    }

//    internal void SetPath()
//    {
//        // Reset tile colors
//        foreach (var t in gameTiles)
//        {
//            t.SetPathColor(false);
//        }

//        foreach (var spawn in pathToGoal)
//        {
//            if (spawn.spawnTile == null)
//            {
//                Debug.LogError("Spawn tile is null in pathToGoal list.");
//                continue;
//            }

//            // Check cache
//            var cacheKey = (spawn.spawnTile, endTile);
//            if (cachedPaths.ContainsKey(cacheKey))
//            {
//                spawn.pathToGoal = new List<GameTiles>(cachedPaths[cacheKey]);
//                Debug.Log("Using cached path.");
//                continue;
//            }

//            spawn.pathToGoal.Clear();
//            var path = PathFinding(spawn.spawnTile, endTile);

//            if (path == null)
//            {
//                Debug.LogError($"PathFinding returned null for spawnTile {spawn.spawnTile.name} and endTile {endTile.name}");
//                continue;
//            }

//            var tile = endTile;
//            while (tile != null)
//            {
//                spawn.pathToGoal.Add(tile);
//                tile.SetPathColor(true);
//                tile = path[tile];
//            }

//            // Cache the path
//            cachedPaths[cacheKey] = new List<GameTiles>(spawn.pathToGoal);
//            Debug.Log($"Path Created and count: {spawn.pathToGoal.Count}");
//        }
//    }

//    private Dictionary<GameTiles, GameTiles> PathFinding(GameTiles sourceTile, GameTiles targetTile)
//    {
//        return PathFindingAStar(sourceTile, targetTile);
//    }

//    private Dictionary<GameTiles, GameTiles> PathFindingAStar(GameTiles sourceTile, GameTiles targetTile)
//    {
//        var openSet = new PriorityQueue<GameTiles>();
//        var cameFrom = new Dictionary<GameTiles, GameTiles>();
//        var gScore = new Dictionary<GameTiles, int>();
//        var fScore = new Dictionary<GameTiles, int>();

//        foreach (var tile in gameTiles)
//        {
//            gScore[tile] = int.MaxValue;
//            fScore[tile] = int.MaxValue;
//        }

//        gScore[sourceTile] = 0;
//        fScore[sourceTile] = Heuristic(sourceTile, targetTile);

//        openSet.Enqueue(sourceTile, fScore[sourceTile]);

//        while (openSet.Count > 0)
//        {
//            var current = openSet.Dequeue();

//            if (current == targetTile)
//            {
//                return ReconstructPath(cameFrom, current);
//            }

//            foreach (var neighbor in FindNeighbor(current))
//            {
//                if (neighbor.IsBloced)
//                    continue;

//                int tentativeGScore = gScore[current] + 1; // Adjust cost for dangerous/slow tiles if needed

//                if (tentativeGScore < gScore[neighbor])
//                {
//                    cameFrom[neighbor] = current;
//                    gScore[neighbor] = tentativeGScore;
//                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, targetTile);

//                    if (!openSet.Contains(neighbor))
//                    {
//                        openSet.Enqueue(neighbor, fScore[neighbor]);
//                    }
//                }
//            }
//        }

//        return null; // No path found
//    }

//    private Dictionary<GameTiles, GameTiles> ReconstructPath(Dictionary<GameTiles, GameTiles> cameFrom, GameTiles current)
//    {
//        var path = new Dictionary<GameTiles, GameTiles>();

//        while (cameFrom.ContainsKey(current))
//        {
//            var prev = cameFrom[current];
//            path[current] = prev;
//            current = prev;
//        }

//        return path;
//    }

//    private int Heuristic(GameTiles a, GameTiles b)
//    {
//        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
//    }

//    private List<GameTiles> FindNeighbor(GameTiles u)
//    {
//        var neighbors = new List<GameTiles>();

//        if (u.X - 1 >= 0) neighbors.Add(gameTiles[u.Y, u.X - 1]);
//        if (u.X + 1 < ColCount) neighbors.Add(gameTiles[u.Y, u.X + 1]);
//        if (u.Y - 1 >= 0) neighbors.Add(gameTiles[u.Y - 1, u.X]);
//        if (u.Y + 1 < RowCount) neighbors.Add(gameTiles[u.Y + 1, u.X]);

//        return neighbors;
//    }

//    internal void Reset()
//    {
//        spawnTile.Clear();
//        cachedPaths.Clear();
//    }
//}


