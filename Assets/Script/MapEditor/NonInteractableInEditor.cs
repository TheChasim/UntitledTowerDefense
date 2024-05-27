using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NonInteractableInEditor : MonoBehaviour
{
    void Awake()
    {
        // Set the HideFlags to NotEditable to make the object non-interactable in the editor
        gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
    }

    void Reset()
    {
        // Set the HideFlags to NotEditable to make the object non-interactable in the editor
        gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;
    }
}
