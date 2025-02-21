using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MovingCam : MonoBehaviour
{
    [SerializeField] float zoomSensitivity = 0.5f;
    [SerializeField] float minZoom = 1;
    [SerializeField] float maxZoom = 10;

    [SerializeField] float moveSpeed;
    Vector2 moveInput;
    float zoomValue;

    GameObject cam;

    private void Start()
    {
        cam = FindObjectOfType<Camera>().gameObject;
    }

    private void Update()
    {
        HandleZoom();
        HandleMovement();

    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 isoMove = (Quaternion.Euler(0, 45, 0) * moveInput) * moveSpeed;
        cam.transform.position += isoMove * Time.deltaTime;
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
        if(context.canceled)
        {
            zoomValue = 0;
        }
    }

    internal void OnMove(InputAction.CallbackContext context)
    {
       moveInput = context.ReadValue<Vector2>();
    }
}
