using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosBackground : MonoBehaviour
{
    [SerializeField] Vector3 Pos;
    [SerializeField] Vector3 Scale;

    private void Start()
    {
        this.transform.position = Pos;
        this.transform.localScale = Scale;
    }
}
