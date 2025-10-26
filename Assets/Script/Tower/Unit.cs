using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    internal GameObject target;
    internal float range;
    internal float damege = 1f;
    internal float cooldown = 1f;
    internal float healt = 5f;
    internal float speed = 5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed*Time.fixedDeltaTime);
        }
    }
}
