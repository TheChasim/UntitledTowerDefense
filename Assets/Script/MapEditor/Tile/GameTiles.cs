using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GameTiles : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler, IPointerClickHandler
{
    [Header("Tille Infos")]
    internal bool IsSelected = false;
    internal bool IsSpawn = false;
    internal bool IsEnd = false;
    internal bool IsBloced = false;
    internal bool IsSlowing = false;
    internal bool IsDamaging = false;

    [Header("Cout de deplacement")]
    [SerializeField] internal float normalCost = 1;
    [SerializeField] internal float damageCost = 3;
    [SerializeField] internal float slowingCost = 1.5f;
    [SerializeField] internal float cost;
    [SerializeField] internal float DamageAmout = 0.5f;
    [SerializeField] internal float SlowingAmout = 2f;

    [Space]
    [Header("General Info")]
    [SerializeField] internal Vector3 worldPosition;
    [SerializeField] internal int gridX, gridY;
    [Space]

    [Header("Direction")]
    //[SerializeField] internal Vector3 flowDirection = Vector3.zero;
    [SerializeField] internal GameTiles nextTile;
    private LineRenderer lineRenderer; // Affichage du Flow Field
    [Space]

    [Header("Sprite Setting")]
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer SelectedRenderer;
    public SpriteRenderer spriteSpawn;
    public SpriteRenderer spriteEnd;
    public SpriteRenderer SlowingRenderer;
    public SpriteRenderer WallRenderer;
    public SpriteRenderer DamagingRenderer;
    private Color originalColor;

    public int X { get; internal set; }
    public int Y { get; internal set; }

    internal void SetValue(Vector3 newWorldPos, int newX, int newY)
    {
        worldPosition = newWorldPos;
        gridX = newX;
        gridY = newY;
    }
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

    async public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsBloced && !GameManager.Instance.deleteTower)
        {
            IsBloced = true;

            if (true) //Changer ca pour assurer que au moins un chemin est disponible depuis le spawn jusqu'a l'arriver
            {
                Debug.Log("Chemin trouvé");

                // Placer la tour
                TowerSpawning.Instance.SpawnTower();
                Vector2Int position = new Vector2Int((int)GameManager.Instance.TargetTile.transform.position.x,
                                                     (int)GameManager.Instance.TargetTile.transform.position.z);

                GameManager.Instance.UpdateFlowFieldAround(position);
            }
            else
            {
                Debug.LogWarning("Chemin impossible");
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

    internal void SetCost()
    {
        if(IsBloced)
        {
            cost = float.MaxValue;
        }
        else
        {
            cost = 10000;
        }
    }
}
