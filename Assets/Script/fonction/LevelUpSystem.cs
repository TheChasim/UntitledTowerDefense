using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelUpSystem : MonoBehaviour
{
    bool up;

    internal void setUp()
    {
        up = true;
    }

    internal void setDowwn()
    {
        up = false;
    }


    public void OnPower()
    {
        if (up)
        {
            Debug.Log("Power up");
        }
        else
        {
            Debug.Log("Power down");
        }

    }

    public void OnRange()
    {
        if (up)
        {
            Debug.Log("range up");
        }
        else
        {
            Debug.Log("Range down");
        }
    }

    public void OnColdown()
    {

        if (up)
        {
            Debug.Log("Coldown up");
        }
        else
        {
            Debug.Log("Coldown down");
        }
    }

}
