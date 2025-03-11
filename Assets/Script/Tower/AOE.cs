using UnityEngine;

public class AOE : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyAI>())
        {
            GetComponentInParent<Tower>(). targets.Add(other.gameObject.GetComponent<EnemyAI>());           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyAI>())
        {
            GetComponentInParent<Tower>().targets.Remove(other.gameObject.GetComponent<EnemyAI>());
        }
    }
}
