using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningEnemy : MonoBehaviour
{
    [SerializeField] float timeBetweenEnemy = 0.5f;
    [SerializeField] float timeBetweenWave = 5f;
    [SerializeField] List<GameObject> enemyAIPrefab = new List<GameObject>();
    public int currentEnemyAmount = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(SpawnEnemyCoroutine());
        }

        currentEnemyAmount = EnemyAI.enemyAIList.Count;
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        var enemy = Instantiate(enemyAIPrefab[Random.Range(0, enemyAIPrefab.Count)],
                                GameManager.spawnTiles[Random.Range(0, GameManager.spawnTiles.Count)].transform.position,
                                Quaternion.identity);

        //enemy.GetComponent<EnemyAI>().SetPath(PathFinder.pathToGoal);

        yield return new WaitForSeconds(timeBetweenEnemy);
        StartCoroutine(SpawnEnemyCoroutine());
    }
}
