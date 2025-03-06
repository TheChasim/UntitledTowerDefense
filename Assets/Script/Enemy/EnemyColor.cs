using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColor : MonoBehaviour
{
    [SerializeField] internal Gradient colorGradiant;
    //[SerializeField] Material colorGradiantShader;

    private void Start()
    {
        //colorGradiantShader.SetColor("_SlimeNewColor", colorGradiant.Evaluate(Random.Range(0.0f, 1f)));

        foreach (SpriteRenderer render in GetComponents<SpriteRenderer>()) 
        {
            render.material.SetColor("_SlimeNewColor", colorGradiant.Evaluate(Random.Range(0.0f, 1f)));
        }
    }

}
    