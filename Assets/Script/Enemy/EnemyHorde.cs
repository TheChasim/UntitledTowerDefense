using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

public class EnemyHorde : MonoBehaviour
{
    Healt healt;
    Horde horde;

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

        if(horde.enemieHorde.Count != math.ceil(healt.gethealt()))
        {
            GameObject remove = horde.enemieHorde.Last();
            //Debug.Log(remove.name + " has die");
            horde.enemieHorde.Remove(remove);
            Destroy(remove);
        }
    }
}
