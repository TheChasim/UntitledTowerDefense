using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MovingCam : MonoBehaviour
{
    [SerializeField] float zoomSensitivity = 0.5f;
    [SerializeField] float minZoom = 1;
    [SerializeField] float maxZoom = 10;
    float zoomValue;

    GameObject cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>().gameObject;
    }

    private void Update()
    {
        HandleZoom();

    }

    private void HandleZoom()
    {
        cam.GetComponent<Camera>().orthographicSize += zoomValue;

        cam.GetComponent<Camera>().orthographicSize = Mathf.Clamp(cam.GetComponent<Camera>().orthographicSize, minZoom, maxZoom);
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            zoomValue = context.ReadValue<Vector2>().y* zoomSensitivity;
        }
        if(context.canceled)
        {
            zoomValue = 0;
        }
    }
}
