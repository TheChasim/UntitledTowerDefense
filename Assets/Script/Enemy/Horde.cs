using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Horde : MonoBehaviour
{
    [SerializeField] bool isAHorde = false;
    [SerializeField] List<Sprite> sprite = new List<Sprite>();
    internal List<GameObject> enemieHorde = new List<GameObject>();

    Healt healt;

    private void Awake()
    {
        healt = GetComponent<Healt>();
        int numHorde = (int)math.round(healt.GetMaxHealt());
        GameObject emptyObject = new GameObject("EmptyObject");

        if (isAHorde)
        {
            GetComponent<SpriteRenderer>().enabled = false;

            for (int i = 0; i < numHorde; i++)
            {

                GameObject newEnemie = Instantiate(emptyObject, Vector3.zero, Quaternion.identity);

                enemieHorde.Add(newEnemie);

                newEnemie.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                newEnemie.transform.parent = transform;
                newEnemie.transform.position = new Vector3(UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(0f, 1f) , 1f);
                
                newEnemie.AddComponent<EnemyHorde>();
                newEnemie.AddComponent<SpriteRenderer>();
                newEnemie.GetComponent<SpriteRenderer>().enabled = true;
                newEnemie.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
                newEnemie.GetComponent<SpriteRenderer>().sprite = sprite[UnityEngine.Random.Range(0, sprite.Count)];

            }
        }
    }

    internal void romoveEnemy()
    {
       ;
    }
}
