using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float timetoDestroy = 1f;

    // Update is called once per frame
    void Update()
    {
        timetoDestroy -= Time.deltaTime;
        if(timetoDestroy < 0f )
        {
            Destroy(gameObject);
        }
    }
}
