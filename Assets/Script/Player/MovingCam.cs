using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class MovingCam : MonoBehaviour
{
    [SerializeField] float zoomSensitivity = 0.5f;
    [SerializeField] float roationSpeed = 50f;
    [SerializeField] float minZoom = 1;
    [SerializeField] float maxZoom = 10;

    [SerializeField] float moveSpeed;
    Vector2 moveInput;
    float zoomValue;
    Vector3 target;

    GameObject cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>().gameObject;
        target = new Vector3(MapLoading.target.x, 0, MapLoading.target.y);
    }

    private void Update()
    {
        HandleZoom();
        HandleMovement();

    }

    private void HandleMovement()
    {
        //obtien le movent 
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        //transforme le movent en iso
        float cameraY = Camera.main.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, cameraY, 0);
        Vector3 isoMove = rotation * move;

        // Applique la vitesse et empeche tout mouvement vertical
        cam.transform.position += isoMove * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        cam.GetComponent<Camera>().orthographicSize += zoomValue * Time.deltaTime;

        cam.GetComponent<Camera>().orthographicSize = Mathf.Clamp(cam.GetComponent<Camera>().orthographicSize, minZoom, maxZoom);
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            zoomValue = -context.ReadValue<Vector2>().y * zoomSensitivity;
        }
        if (context.canceled)
        {
            zoomValue = 0;
        }
    }

    internal void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    internal void OnRotateCam(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log(context.ReadValue<float>());
            Camera.main.transform.RotateAround(target, Vector3.up, context.ReadValue<float>()* roationSpeed * Time.deltaTime);

        }

    }
}
