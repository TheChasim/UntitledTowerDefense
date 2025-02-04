using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldPathfinding : MonoBehaviour
{
    [Header("Show Direction")]
    [SerializeField] bool ShowArrow;
    public GameObject lineRendererPrefab;

    [SerializeField] float cellSize = 1f;
    private int col;
    private int row;

    public void SetFlowField(GameTiles[,] gameTile)
    {
        col = gameTile.GetLength(0);
        row = gameTile.GetLength(1);

        GenerateGrid(gameTile);
        GenerateIntegrationField(gameTile);
        GenerateFlowField(gameTile);

        if (ShowArrow)
        {
            PlaceLineRenderers(gameTile);
            UpdateAllLineRenderers(gameTile);
        }
    }

    void GenerateGrid(GameTiles[,] gameTile)
    {
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, 0, y * cellSize); // Y=0 pour �tre sur le sol
                gameTile[x, y].SetValue(worldPos, x, y);
            }
        }
    }

    void GenerateIntegrationField(GameTiles[,] gameTile)
    {
        Queue<GameTiles> nodesQueue = new Queue<GameTiles>();
        GameTiles targetNode = null;

        // Initialiser toutes les cellules � un co�t tr�s �lev�
        foreach (GameTiles tile in gameTile)
        {
            tile.cost = float.MaxValue; // Par d�faut, co�t max
            if (tile.IsEnd)
            {
                targetNode = tile;
            }
        }

        if (targetNode == null)
        {
            Debug.LogError("Aucune case cible trouv�e !");
            return;
        }

        targetNode.cost = 0;
        nodesQueue.Enqueue(targetNode);

        while (nodesQueue.Count > 0)
        {
            GameTiles current = nodesQueue.Dequeue();

            foreach (GameTiles neighbor in GetNeighbors(current, gameTile))
            {
                if (neighbor.cost == float.MaxValue) continue; // Ignorer obstacles

                float newCost = current.cost + 1;
                if (newCost < neighbor.cost)
                {
                    neighbor.cost = newCost;
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

            // V�rifier si la position est dans la grille
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
            float lowestCost = float.MaxValue;

            foreach (GameTiles neighbor in neighbors)
            {
                if (neighbor.cost < lowestCost)
                {
                    lowestCost = neighbor.cost;
                    bestNeighbor = neighbor;
                }
            }

            if (bestNeighbor != null)
            {
                // Utilisation de `Vector3` pour s'assurer que l'axe Y reste � 0 (pas de distorsion)
                Vector3 flowDir = (bestNeighbor.worldPosition - tile.worldPosition).normalized;
                tile.flowDirection = new Vector3(flowDir.x, 0, flowDir.z); // Ignore Y
            }
        }
    }

    public void PlaceLineRenderers(GameTiles[,] gameTile)
    {
        foreach (GameTiles tile in gameTile)
        {
            tile.SetLineRenderer(lineRendererPrefab, transform);
        }
    }

    public void UpdateAllLineRenderers(GameTiles[,] gameTile)
    {
        GameTiles targetNode = null;

        foreach (GameTiles tile in gameTile)
        {
            if (tile.IsEnd)
            {
                targetNode = tile;
                break;
            }
        }

        if (targetNode == null)
        {
            Debug.LogError("Pas de case cible trouv�e !");
            return;
        }

        foreach (GameTiles tile in gameTile)
        {
            tile.UpdateLineRenderer(targetNode);
        }
    }
}

