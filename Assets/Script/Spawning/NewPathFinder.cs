using System;
using System.Collections.Generic;
using UnityEngine;

// Implémentation de la PriorityQueue (Min-Heap)
public class PriorityQueue<T>
{
    private List<(T item, float priority)> heap = new List<(T, float)>();

    public int Count => heap.Count;

    public void Enqueue(T item, float priority)
    {
        heap.Add((item, priority));
        HeapifyUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("Queue is empty");

        var root = heap[0].item;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);

        return root;
    }

    public bool Contains(T item)
    {
        return heap.Exists(x => EqualityComparer<T>.Default.Equals(x.item, item));
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].priority >= heap[parent].priority) break;

            (heap[index], heap[parent]) = (heap[parent], heap[index]);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while (true)
        {
            int leftChild = index * 2 + 1;
            int rightChild = index * 2 + 2;
            int smallest = index;

            if (leftChild <= lastIndex && heap[leftChild].priority < heap[smallest].priority)
                smallest = leftChild;

            if (rightChild <= lastIndex && heap[rightChild].priority < heap[smallest].priority)
                smallest = rightChild;

            if (smallest == index) break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }
}

//liste de liste pour les chemin
public class NewPathToGoal
{
    internal GameTiles spawnTile;
    internal List<GameTiles> pathToGoal = new List<GameTiles>();
    internal List<GameTiles> tempPathToGoal = new List<GameTiles>();

    //internal int leght;
}

// Script principal du Pathfinder
public class NewPathFinder : MonoBehaviour
{
    GameTiles[,] gameTile;
    //internal List<GameTiles> spawnTile;
    internal GameTiles endTile;
    static internal List<PathToGoal> pathToGoal = new List<PathToGoal>();
    List<PathToGoal> tempPathToGoal = new List<PathToGoal>();
    private int col = 0;
    private int row = 0;

    // Initialisation de la grille
    internal void SetValue(GameTiles[,] newGameTiles, List<GameTiles> newSpawnTile, GameTiles newEndTile, int newCol, int newRow)
    {
        gameTile = newGameTiles;
        //spawnTile = newSpawnTile;
        endTile = newEndTile;
        col = newCol;
        row = newRow;

        foreach (var spawn in newSpawnTile)
        {
            PathToGoal tempPath = new PathToGoal();
            tempPath.spawnTile = spawn;
            pathToGoal.Add(tempPath);
        }
    }

    // Algorithme A* pour trouver le chemin
    public List<GameTiles> FindPathAStar()
    {

        foreach (var spawn in pathToGoal)
        {
            var openSet = new PriorityQueue<GameTiles>();
            var cameFrom = new Dictionary<GameTiles, GameTiles>();

            var gScore = new Dictionary<GameTiles, float>();
            var fScore = new Dictionary<GameTiles, float>();

            // Initialisation des scores
            foreach (var tile in gameTile)
            {
                gScore[tile] = float.MaxValue;
                fScore[tile] = float.MaxValue;
            }

            gScore[spawn.spawnTile] = 0;
            fScore[spawn.spawnTile] = HeuristicCostEstimate(spawn.spawnTile, endTile);

            openSet.Enqueue(spawn.spawnTile, fScore[spawn.spawnTile]);

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();

                // Si nous avons atteint la destination, reconstruire le chemin
                if (current == endTile)
                {
                    spawn.pathToGoal = ReconstructPath(cameFrom, current);
                    //return ReconstructPath(cameFrom, current);
                }

                foreach (var neighbor in FindNeighbor(current))
                {
                    // Calculer le coût pour le voisin
                    float tentativeGScore = gScore[current] + GetTraversalCost(neighbor);

                    if (tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, endTile);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                        }
                    }
                }
            }
        }

        Debug.Log("Change Path color");
        SetPathColor();
        // Retourner une liste vide si aucun chemin n'a été trouvé
        return new List<GameTiles>();
    }

    public List<GameTiles> FindPathAStar(GameTiles currentTille)
    {
        var openSet = new PriorityQueue<GameTiles>();
        var cameFrom = new Dictionary<GameTiles, GameTiles>();

        var gScore = new Dictionary<GameTiles, float>();
        var fScore = new Dictionary<GameTiles, float>();

        // Initialisation des scores
        foreach (var tile in gameTile)
        {
            gScore[tile] = float.MaxValue;
            fScore[tile] = float.MaxValue;
        }

        gScore[currentTille] = 0;
        fScore[currentTille] = HeuristicCostEstimate(currentTille, endTile);

        openSet.Enqueue(currentTille, fScore[currentTille]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            // Si nous avons atteint la destination, reconstruire le chemin
            if (current == endTile)
            {
                //spawn.pathToGoal = ReconstructPath(cameFrom, current);
                return ReconstructPath(cameFrom, current);
            }

            foreach (var neighbor in FindNeighbor(current))
            {
                // Calculer le coût pour le voisin
                float tentativeGScore = gScore[current] + GetTraversalCost(neighbor);

                if (tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, endTile);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }
        }

        Debug.Log("Change Path color");
        SetPathColor();
        // Retourner une liste vide si aucun chemin n'a été trouvé
        return new List<GameTiles>();
    }

    private void SetPathColor()
    {
        foreach (var t in gameTile)
        {
            t.SetPathColor(false);
            Debug.Log("Reset Color");
        }

        foreach (var path in pathToGoal)
        {
            Debug.Log(path.pathToGoal.Count);
            //for (int i = 0; i != path.pathToGoal.Count; i++)
            //{
            //    Debug.Log("paint path");
            //    path.pathToGoal[i].SetPathColor(true);
            //}
            foreach (var tile in path.pathToGoal)
            {
                tile.SetPathColor(true);
            }
        }

    }

    // Heuristique basée sur la distance de Manhattan
    private float HeuristicCostEstimate(GameTiles from, GameTiles to)
    {
        return Mathf.Abs(from.X - to.X) + Mathf.Abs(from.Y - to.Y);
    }

    // Définir le coût pour traverser une tuile
    private float GetTraversalCost(GameTiles tile)
    {
        if (tile.IsSlowing) return 3.0f; // Coût élevé pour les cases lentes
        if (tile.IsDamaging) return 10.0f; // Coût très élevé pour les cases dangereuses
        return 1.0f; // Coût normal
    }

    // Reconstruire le chemin à partir de la cible
    private List<GameTiles> ReconstructPath(Dictionary<GameTiles, GameTiles> cameFrom, GameTiles current)
    {
        var path = new List<GameTiles>();
        while (current != null)
        {
            //current.SetPathColor(true);
            path.Add(current);
            cameFrom.TryGetValue(current, out current);
        }

        path.Reverse();
        return path;
    }

    // Trouver les voisins d'une tuile
    private List<GameTiles> FindNeighbor(GameTiles u)
    {
        var neighbors = new List<GameTiles>();
        var directions = new (int dx, int dy)[]
        {
            (-1, 0), (1, 0), (0, -1), (0, 1) // Gauche, Droite, Haut, Bas
        };

        foreach (var (dx, dy) in directions)
        {
            int nx = u.X + dx;
            int ny = u.Y + dy;

            if (nx >= 0 && nx < col && ny >= 0 && ny < row)
            {
                var neighbor = gameTile[ny, nx];
                if (!neighbor.IsBloced) neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    internal List<int> GetLength()
    {
        List<int> lenght = new List<int>();
        //FindTempPathAStar();

        foreach (var path in pathToGoal)
        {
            Debug.Log(path.pathToGoal.Count);
            lenght.Add(path.pathToGoal.Count);
        }

        return lenght;
    }

    //public List<GameTiles> FindTempPathAStar()
    //{
    //    foreach (var t in gameTile)
    //    {
    //        t.SetPathColor(false);
    //    }

    //    foreach (var spawn in tempPathToGoal)
    //    {
    //        var openSet = new PriorityQueue<GameTiles>();
    //        var cameFrom = new Dictionary<GameTiles, GameTiles>();

    //        var gScore = new Dictionary<GameTiles, float>();
    //        var fScore = new Dictionary<GameTiles, float>();

    //        // Initialisation des scores
    //        foreach (var tile in gameTile)
    //        {
    //            gScore[tile] = float.MaxValue;
    //            fScore[tile] = float.MaxValue;
    //        }

    //        gScore[spawn.spawnTile] = 0;
    //        fScore[spawn.spawnTile] = HeuristicCostEstimate(spawn.spawnTile, endTile);

    //        openSet.Enqueue(spawn.spawnTile, fScore[spawn.spawnTile]);

    //        while (openSet.Count > 0)
    //        {
    //            var current = openSet.Dequeue();

    //            // Si nous avons atteint la destination, reconstruire le chemin
    //            if (current == endTile)
    //            {
    //                spawn.pathToGoal = ReconstructPath(cameFrom, current);
    //                //return ReconstructPath(cameFrom, current);
    //            }

    //            foreach (var neighbor in FindNeighbor(current))
    //            {
    //                // Calculer le coût pour le voisin
    //                float tentativeGScore = gScore[current] + GetTraversalCost(neighbor);

    //                if (tentativeGScore < gScore[neighbor])
    //                {
    //                    cameFrom[neighbor] = current;
    //                    gScore[neighbor] = tentativeGScore;
    //                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, endTile);

    //                    if (!openSet.Contains(neighbor))
    //                    {
    //                        openSet.Enqueue(neighbor, fScore[neighbor]);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    // Retourner une liste vide si aucun chemin n'a été trouvé
    //    return new List<GameTiles>();
    //}
}
