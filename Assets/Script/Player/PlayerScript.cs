using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;
    [SerializeField] Scrollbar hpBar;
    [SerializeField] TMP_Text MoneyText;
    [SerializeField] TMP_Text ExpText;
    [SerializeField] TMP_Text waveText;
    [SerializeField] int BaseHp;
    internal int currentHp;
    internal int bonusHp;
    int totalHp;
    [SerializeField] internal int currentMoney;
    [SerializeField] internal int currentExp;
    [SerializeField] internal int currentWave;

    public static PlayerScript Singleton;

    // Start is called before the first frame update
    void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        //set la taille de hp
        totalHp = currentHp = BaseHp;
        
        hpBar.size = currentHp / totalHp;
        hpText.text = currentHp.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTakeDamege(int damege)
    {   
        math.clamp(currentHp -= damege, 0, 999);

        Debug.Log($"currentHp {currentHp}/{totalHp} TotalHp = {(float)currentHp / totalHp}");
        hpBar.size = (float)currentHp / totalHp;
        hpText.text = currentHp.ToString();

        if(currentHp ==0)
        {
            // mettre la fin de partie ici
        }
    }

    public void AddBonusHp(int  bonusHp)
    {
        totalHp += bonusHp;
    }

}
