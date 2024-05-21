using System.Collections;
using System.Collections.Generic;

public class PathFinder
{
    GameTiles[,] gameTiles;
    internal GameTiles spawnTile;
    internal GameTiles endTile;
    List<GameTiles> pathToGoal = new List<GameTiles>();

    int ColCount = 0;
    int RowCount = 0;

    //constructeur
    public PathFinder(GameTiles[,] NewGametiles,GameTiles NewSpawnTile , GameTiles NewEndTil , int NewCol, int NewRow)
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

        var path = PathFinfing(spawnTile, endTile);
        var tile = endTile;

        while (tile != null)
        {
            pathToGoal.Add(tile);
            tile.SetPathColor(true);
            tile = path[tile];
        }
        //play = true;
        //spwaning.Spawning(spawnTile, pathToGoal);
    }

    private Dictionary<GameTiles, GameTiles> PathFinfing(GameTiles sourceTile, GameTiles targetTile)
    {
        var dist = new Dictionary<GameTiles, int>();
        var prev = new Dictionary<GameTiles, GameTiles>();

        var Q = new List<GameTiles>();

        foreach (var v in gameTiles)
        {
            dist.Add(v, 999);
            prev.Add(v, null);
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

            Q.Remove(u);

            foreach (var v in FindNeighbor(u))
            {
                if (!Q.Contains(v) || v.IsBloced)
                {
                    continue;
                }

                int alt = dist[u] + 1;

                if (alt < dist[v])
                {
                    dist[v] = alt;

                    prev[v] = u;
                }
            }

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
