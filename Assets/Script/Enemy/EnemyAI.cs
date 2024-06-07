using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    static internal HashSet<EnemyAI> enemyAIList = new HashSet<EnemyAI>();

    private void Awake()
    {
        enemyAIList.Add(this);
    }

    internal void SetPath(List<GameTiles> pathToGoal)
    {
        throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        enemyAIList.Remove(this);
    }
}
