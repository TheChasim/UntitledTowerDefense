using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningEnemy : MonoBehaviour
{
    [SerializeField] float timeBetweenEnemy = 0.5f;
    [SerializeField] float timeBetweenWave = 5f;
    [SerializeField] List<GameObject> enemyAIPrefab = new List<GameObject>();
    public int currentEnemyAmount = 0;

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
        Debug.Log("num of spawn tile : " + GameManager.spawnTiles.Count);
        if (enemyAIPrefab == null || enemyAIPrefab.Count == 0)
        {
            Debug.LogError("ERREUR : Aucun prefab d'ennemi défini !");
            yield break;
        }
        var spawnTile = GameManager.spawnTiles[Random.Range(0, GameManager.spawnTiles.Count)];
        var enemy = Instantiate(enemyAIPrefab[Random.Range(0, enemyAIPrefab.Count)],
                                new Vector3(spawnTile.transform.position.x, spawnTile.transform.position.y+0.25f, spawnTile.transform.position.z),
                                Quaternion.identity);

        //enemy.GetComponent<EnemyAI>().currentTile = spawnTile;
        //enemy.GetComponent<EnemyAI>().SetPath(spawnTile);

        yield return new WaitForSeconds(timeBetweenEnemy);
        StartCoroutine(SpawnEnemyCoroutine());
    }
}
