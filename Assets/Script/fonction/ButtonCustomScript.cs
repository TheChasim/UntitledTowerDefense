using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
using System;

public class ButtonCustomScript : MonoBehaviour, IPointerClickHandler
{
    [Header("Size Setting")]
    [SerializeField] Vector3 minSize;
    [SerializeField] Vector3 maxSize;
    [SerializeField] Vector3 sizeIncreasement = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("Stat")]
    [SerializeField] bool Power = false;
    [SerializeField] bool Range = false;
    [SerializeField] bool Coldown = false;

    [SerializeField] LevelUpSystem levelUpSystem;





    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UpSize();
            levelUpSystem.setUp();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            DownSIze();
            levelUpSystem.setDowwn();
        }


    }

    private void UpSize()
    {
        if (transform.localScale.x + sizeIncreasement.x <= maxSize.x)
        {
            transform.localScale = transform.localScale + sizeIncreasement;
        }
    }

    private void DownSIze()
    {
        if (transform.localScale.x - sizeIncreasement.x >= minSize.x)
        {
            transform.localScale = transform.localScale - sizeIncreasement;
        }
    }

}

