using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSelection : MonoBehaviour
{

    [SerializeField] int indextower;

    TowerSpawning spaning;

    void Awake()
    {
        spaning = FindAnyObjectByType<TowerSpawning>().GetComponent<TowerSpawning>();
    }

    public void OnClick()
    {
        spaning.GetComponent<TowerSpawning>().OnSelectIndex(indextower);
    }
}
