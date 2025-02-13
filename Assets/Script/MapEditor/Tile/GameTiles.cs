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

            //bool pathIsValid = GameManager.Instance.GetPathLeght().All(length => length > 2);

            if (true) //Changer ca pour assurer que au moins un chemin est disponible depuis le spawn jusqu'a l'arriver
            {
                Debug.Log("Chemin trouvé");

                // Placer la tour
                TowerSpawning.Instance.SpawnTower();
                //GameManager.Instance.SetPath();
                Vector2Int position = new Vector2Int((int)GameManager.Instance.TargetTile.transform.position.x,
                                                     (int)GameManager.Instance.TargetTile.transform.position.z);

                GameManager.Instance.UpdateFlowFieldAround(position);
                //GameManager.Instance.SetPath();

                //// Paralléliser la mise à jour des chemins pour tous les ennemis
                //Task[] updateTasks = EnemyAI.enemyAIList.Select(enemi => enemi.SetPath()).ToArray();
                //await Task.WhenAll(updateTasks);

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

                //// Paralléliser la mise à jour des chemins pour tous les ennemis
                //Task[] updateTasks = EnemyAI.enemyAIList.Select(enemi => enemi.SetPath()).ToArray();
                //await Task.WhenAll(updateTasks);
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

    // Initialise le LineRenderer
    public void SetLineRenderer(GameObject lineRendererPrefab, Transform parent)
    {
        GameObject lineObj = GameObject.Instantiate(lineRendererPrefab, parent);
        lineRenderer = lineObj.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }
    }

    //// Met à jour la direction et la largeur de la ligne
    //public void UpdateLineRenderer(GameTiles targetNode)
    //{
    //    if (lineRenderer == null || flowDirection == Vector3.zero) return;

    //    // Position de départ (milieu de la tuile)
    //    Vector3 startPos = worldPosition + Vector3.up * 0.1f;
    //    // Position de fin selon la direction
    //    Vector3 endPos = startPos + new Vector3(flowDirection.x, 0, flowDirection.y) * 0.5f;

    //    lineRenderer.SetPosition(0, startPos);
    //    lineRenderer.SetPosition(1, endPos);

    //    // Calcul de l'épaisseur de la ligne en fonction de la distance à la cible
    //    float distanceToTarget = Vector3.Distance(worldPosition, targetNode.worldPosition);
    //    float thickness = Mathf.Clamp(0.05f + (1 - (distanceToTarget / 10f)) * 0.2f, 0.05f, 0.2f); // Max épaisseur = 0.2

    //    lineRenderer.startWidth = thickness;
    //    lineRenderer.endWidth = thickness;

    //    // Changer la couleur en fonction de la proximité
    //    float intensity = Mathf.Clamp01(1 - (distanceToTarget / 10f));
    //    lineRenderer.startColor = new Color(1, intensity * 0.5f, intensity * 0.5f); // Rouge plus intense proche de la cible
    //    lineRenderer.endColor = lineRenderer.startColor;
    //}

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
