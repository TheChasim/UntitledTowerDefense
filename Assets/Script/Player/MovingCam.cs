using System;
using System.Runtime.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

internal enum RotationType
{
    Continue,
    Step,

};

public class MovingCam : MonoBehaviour
{
    [SerializeField] float zoomSensitivity = 0.5f;
    [SerializeField] float rotationSpeed = 50f;  //for rotation continue it will turn by a speed
    [SerializeField] float rotationAngle = 75f;  //for rotation by step it will rotate by an specefic angle
    [SerializeField] RotationType rotationType;
    [SerializeField] float minZoom = 1;
    [SerializeField] float maxZoom = 10;

    [SerializeField] float moveSpeed;
    Vector2 moveInput;
    float rotationValue;
    float zoomValue;
    Vector3 target;

    GameObject cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>().gameObject;
        target = new Vector3(MapLoading.target.x, 0, MapLoading.target.y);
    }

    private void FixedUpdate()
    {
        HandleZoom();
        HandleMovement();

        if(rotationType == RotationType.Continue)
        {
            Camera.main.transform.RotateAround(target, Vector3.up, rotationValue * rotationSpeed * Time.fixedDeltaTime);
        }

        Debug.DrawLine(Camera.main.transform.position, target, Color.red);

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

        Camera.main.fieldOfView += zoomValue * Time.deltaTime;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 10, 60);


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
        rotationValue = context.ReadValue<float>();

        if (rotationType == RotationType.Step)
        {
            Debug.Log(context.ReadValue<float>());
            Camera.main.transform.RotateAround(target, Vector3.up, rotationValue * rotationAngle);
            //Camera.main.transform.rotation.y += rotationValue * rotationAngle;

        }

    }
}
