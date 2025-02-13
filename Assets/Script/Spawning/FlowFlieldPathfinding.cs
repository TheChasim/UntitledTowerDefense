using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldPathfinding : MonoBehaviour
{
    [SerializeField] float cellSize = 1f;
    [SerializeField] int changingRadius = 3;
    private int col;
    private int row;

    public void SetFlowField(GameTiles[,] gameTile)
    {
        col = gameTile.GetLength(0);
        row = gameTile.GetLength(1);

        GenerateGrid(gameTile);
        GenerateIntegrationField(gameTile);
        GenerateFlowField(gameTile);

    }

    void GenerateGrid(GameTiles[,] gameTile)
    {
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, 0, y * cellSize); // Y=0 pour être sur le sol
                gameTile[x, y].SetValue(worldPos, x, y);
            }
        }
    }

    void GenerateIntegrationField(GameTiles[,] gameTile)
    {
        Queue<GameTiles> nodesQueue = new Queue<GameTiles>();
        GameTiles targetNode = null;

        // Initialiser toutes les cellules à un coût très élevé
        foreach (GameTiles tile in gameTile)
        {
            tile.SetCost();
            if (tile.cost != float.MaxValue)
            {
                tile.cost = 10000;
            }
            if (tile.IsEnd)
            {
                targetNode = tile;
            }
        }

        if (targetNode == null)
        {
            Debug.LogError("Aucune case cible trouvée !");
            return;
        }

        targetNode.cost = 0f;

        nodesQueue.Enqueue(targetNode);

        while (nodesQueue.Count > 0)
        {
            GameTiles current = nodesQueue.Dequeue();

            foreach (GameTiles neighbor in GetNeighbors(current, gameTile))
            {
                //neighbor.SetCost();
                if (neighbor.cost == float.MaxValue) continue; // Ignorer obstacles


                float newCost;

                //set le cost de la tuille 
                if (current.IsDamaging)
                { newCost = current.cost + 3; }
                else if (current.IsSlowing)
                { newCost = current.cost + 2; }
                else if (current.IsBloced)
                { newCost = float.MaxValue; }
                else
                { newCost = current.cost + 1; }
          
                if (newCost < neighbor.cost)
                {         
                    neighbor.cost = newCost;
                    neighbor.nextTile = current;
                    nodesQueue.Enqueue(neighbor);
                }
            }
        }
    }

    public List<GameTiles> GetNeighbors(GameTiles tile, GameTiles[,] gameTile)
    {
        List<GameTiles> neighbors = new List<GameTiles>();
        int x = tile.gridX;
        int y = tile.gridY;

        // Directions possibles (haut, bas, gauche, droite)
        int[,] directions = new int[,]
        {
            { 0, 1 },  // Haut
            { 1, 0 },  // Droite
            { 0, -1 }, // Bas
            { -1, 0 }  // Gauche
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = x + directions[i, 0];
            int newY = y + directions[i, 1];

            // Vérifier si la position est dans la grille
            if (newX >= 0 && newX < col && newY >= 0 && newY < row)
            {
                neighbors.Add(gameTile[newX, newY]);
            }
        }
        return neighbors;
    }

    public void GenerateFlowField(GameTiles[,] gameTile)
    {
        foreach (GameTiles tile in gameTile)
        {
            List<GameTiles> neighbors = GetNeighbors(tile, gameTile);
            GameTiles bestNeighbor = null;
            float lowestCost = tile.cost;

            foreach (GameTiles neighbor in neighbors)
            {
                if (neighbor.cost < lowestCost)
                {
                    lowestCost = neighbor.cost;
                    bestNeighbor = neighbor;
                }
            }
        }
    }

    // === OPTIMISATION : Recalcule seulement une partie du Flow Field ===
    public void UpdateFlowFieldAround(Vector2Int position, GameTiles[,] gameTile)
    {
        int startX = Mathf.Max(0, position.x - changingRadius);
        int startY = Mathf.Max(0, position.y - changingRadius);
        int endX = Mathf.Min(col - 1, position.x + changingRadius);
        int endY = Mathf.Min(row - 1, position.y + changingRadius);

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                gameTile[x, y].SetCost();
            }
        }

        GenerateIntegrationField(gameTile);
        GenerateFlowField(gameTile);
    }
}

