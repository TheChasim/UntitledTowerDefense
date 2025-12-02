using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BillBoard : MonoBehaviour
{
    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;
    private void Awake()
    {
        originalRotation = transform.rotation.eulerAngles;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        //transform.rotation = Camera.main.transform.rotation;

        transform.forward = Camera.main.transform.forward;

        // Modify the rotation in Euler space to lock certain dimensions.
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) { rotation.x -= transform.rotation.eulerAngles.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
        transform.rotation = Quaternion.Euler(rotation);
    }
}
