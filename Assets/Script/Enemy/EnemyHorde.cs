using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using System;

public class EnemyHorde : MonoBehaviour
{
    Healt healt;
    Horde horde;
    internal GameObject deadParticul;

    private void Start()
    {
        healt = GetComponentInParent<Healt>();
        horde = GetComponentInParent<Horde>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        if (horde.enemieHorde.Count > math.ceil(healt.gethealt()))
        {
            GameObject remove = horde.enemieHorde.Last();
            //Debug.Log(remove.name + " has die");
            //StartCoroutine(IsDead(remove));

            Instantiate(deadParticul, remove.transform.position, quaternion.identity);
            horde.enemieHorde.Remove(remove);
            Destroy(remove);
        }
    }

    IEnumerator IsDead(GameObject remove)
    {
        deadParticul.GetComponent<ParticleSystem>().Play();

        remove.GetComponent<SpriteRenderer>().enabled = false;
        remove.transform.parent = null;

        yield return new WaitForSeconds(1);

        
        horde.enemieHorde.Remove(remove);
        Destroy(remove);
    }

}
