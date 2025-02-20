using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [SerializeField] MovingCam cam;

    public void OnZoom(InputAction.CallbackContext context)
    {
        cam.OnZoom(context);
    }
}
