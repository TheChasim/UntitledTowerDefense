using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NonInteractableInEditor : MonoBehaviour
{
    public bool notEditable;
    public bool hideInHierarchy;

    internal void OnReset()
    {
        notEditable = false;
        hideInHierarchy = false;
    }

    void Awake()
    {
        if (notEditable)
        {
            gameObject.hideFlags = HideFlags.NotEditable;
        }

        if (hideInHierarchy)
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    void Reset()
    {
        if (notEditable)
        {
            gameObject.hideFlags = HideFlags.NotEditable;
        }

        if (hideInHierarchy)
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }


}
