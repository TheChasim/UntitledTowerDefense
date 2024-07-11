using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealtBar : MonoBehaviour
{
    private Vector3 maxSizeHpBar;
    private Vector3 curentSize;


    void Start()
    {
        maxSizeHpBar = transform.localScale;
        curentSize = transform.localScale;
    }

    internal void SetHealtBar(float currentHealt, float maxHealt)
    {
        curentSize.x = (currentHealt / maxHealt) * maxSizeHpBar.x;

        transform.localScale = curentSize;
    }

}
