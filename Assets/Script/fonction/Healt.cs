using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Healt : MonoBehaviour
{
    //healt info
    [SerializeField] internal float maxHealt;
    [SerializeField] GameObject txtDamage;
    public float curentHealt;
    EnemyAI enemy;

    //healt bar
    [SerializeField] HealtBar healtBar;
    private Vector3 maxSizeHpBar;
    private Vector3 curentSize;

    internal float GetMaxHealt()
    {
        return maxHealt;
    }

    internal float gethealt()
    {
        return curentHealt;
    }

    private void Start()
    {
        enemy = GetComponent<EnemyAI>();
        curentHealt = maxHealt;
    }

    internal void OnTakeDamage(float damage)
    {
        //affiche un effet de damege
        txtDamage.GetComponent<TextMeshPro>().text = damage.ToString();
        Instantiate(txtDamage, transform.position+new Vector3(0,0.5f,0), Quaternion.identity);

        curentHealt = Mathf.Clamp(curentHealt - damage,0,maxHealt);
        //healtBar.SetHealtBar(curentHealt, maxHealt);

        if (curentHealt == 0)
        {
            enemy.IsDead();
        }
        
    }

    internal void OnTakeHealing(float healt)
    {
        curentHealt = Mathf.Clamp(curentHealt + healt, 0, maxHealt);
        //healtBar.SetHealtBar(curentHealt, maxHealt);
    }
}
