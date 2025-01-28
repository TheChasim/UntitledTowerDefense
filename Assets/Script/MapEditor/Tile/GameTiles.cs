using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool IsBloced = false;
    internal bool IsSlowing = false;
    internal bool IsDamaging = false;

    [SerializeField] internal float DamageAmout = 0.5f;
    [SerializeField] internal float SlowingAmout = 2f;


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

            bool pathIsValid = GameManager.Instance.GetPathLeght().All(length => length > 2);

            if (pathIsValid)
            {
                Debug.Log("Chemin trouvé");

                // Placer la tour
                TowerSpawning.Instance.SpawnTower();
                GameManager.Instance.SetPath();

                // Mettre à jour les chemins pour tous les ennemis
                foreach (var enemy in EnemyAI.enemyAIList)
                {
                    enemy.SetPath();
                }
            }
            else
            {
                Debug.Log("Chemin impossible");
                IsBloced = false; // Annuler le blocage
            }
        }
        else if (IsBloced && GameManager.Instance.deleteTower)
        {
            // Trouver la tour proche et la supprimer
            var nearbyTower = Tower.allTourel.FirstOrDefault(tower =>
                Vector3.Distance(this.transform.position, tower.transform.position) < 1);

            if (nearbyTower != null)
            {
                IsBloced = false;

                // Supprimer la tour
                nearbyTower.OnRevome();
                GameManager.Instance.SetPath();

                // Mettre à jour les chemins pour tous les ennemis
                foreach (var enemy in EnemyAI.enemyAIList)
                {
                    enemy.setNewPath();
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
