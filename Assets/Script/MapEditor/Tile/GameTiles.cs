using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTiles : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler, IPointerClickHandler
{
    private Color originalColor;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer SelectedRenderer;
    public SpriteRenderer spriteSpawn;
    public SpriteRenderer spriteEnd;
    public SpriteRenderer SlowingRenderer;
    public SpriteRenderer WallRenderer;
    public SpriteRenderer DamagingRenderer;

    internal bool IsSelected = false;
    internal bool IsSpawn = false;
    internal bool IsEnd = false;

    public  bool IsBloced = false;
    internal bool IsSlowing = false;
    internal bool IsDamaging = false;

    public int X { get; internal set; }
    public int Y { get; internal set; }

    internal void SetComponent()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectedRenderer.enabled = true;
        GameManager.Instance.TargetTile = this;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SelectedRenderer.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsBloced && !GameManager.Instance.deleteTower)
        {
            IsBloced = true;
            bool pathFindWay = true;

            //boucle attraver tout les taille des chemins pour voir s'ils sont possible
            foreach (var leght in GameManager.Instance.GetTempPathLeght())
            {
                //si le chemin est de moins de deux tuile empaiche de placer une tour
                if (leght < 2)
                {
                    pathFindWay = false;
                    break;
                }
            }

            //si tout les chemin sont bon continuer
            if (pathFindWay)
            {
                    TowerSpawning.Instance.SpawnTower();
                

            }
        }

        if (IsBloced && GameManager.Instance.deleteTower)
        {
            foreach (Tower tourel in Tower.allTourel)
            {
                Debug.Log(Vector3.Distance(this.transform.position, tourel.transform.position));
                if (Vector3.Distance(this.transform.position, tourel.transform.position) < 1)
                {
                    IsBloced = false;
                    //player.OnGetMoney(tourel.cost);
                    tourel.OnRevome();
                    break;
                }
            }

        }

    }

    internal void TurnSpawn()
    {
        IsSpawn = !IsSpawn;
        spriteSpawn.enabled = IsSpawn;
    }

    internal void TurnEnd()
    {
        IsEnd = !IsEnd;
        spriteEnd.enabled = IsEnd;
    }

    internal void TurnGrey(float alphaValue)
    {
        Color currentColor = originalColor;
        Color newColor = new Color(currentColor.grayscale, currentColor.grayscale, currentColor.grayscale, 0.5f);
        spriteRenderer.color = newColor;
    }

    internal void TurnBloced()
    {
        IsBloced = !IsBloced;
        WallRenderer.enabled = IsBloced;
    }

    internal void TurnSlow()
    {
        IsSlowing = !IsSlowing;
        SlowingRenderer.enabled = IsSlowing;
    }

    internal void TurnDamaging()
    {
        IsDamaging = !IsDamaging;
        DamagingRenderer.enabled = IsDamaging;
    }

    internal void SetPathColor(bool isPath)
    {
        Color transparentOrange = new Color(1, 0.375f, 0, 0.5f);

        spriteRenderer.color = isPath ? transparentOrange : originalColor;
    }
}
