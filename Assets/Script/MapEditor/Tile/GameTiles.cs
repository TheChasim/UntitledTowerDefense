using System;
using System.Collections.Generic;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        //optien les info de la tuille actuel
        GameTiles tempTile;
        Vector2Int position = new Vector2Int((int)GameManager.Instance.TargetTile.transform.position.x,
                                             (int)GameManager.Instance.TargetTile.transform.position.z);

        //si la tuille n'est pas deja utiliser et n'est pas en supression
        if (CanPlace() && !GameManager.Instance.deleteTower)
        {
            //prend le nextTile et le met en temp au cas on doit lui remmetre
            tempTile = nextTile;
            //rend la tuille bloquer et en suprime le nextTile
            IsBloced = true;
            nextTile = null;

            //uptade le flowField avec la nouvelle tuille bloquer
            GameManager.Instance.UpdateFlowFieldAround(position);

            //si tout les chemin son encore possible continuer
            if (IsPathAvailable(GameManager.Instance.currentGameTiles))
            {
                Debug.Log("Chemin trouvé");
                // Placer la tour
                TowerSpawning.Instance.SpawnTower();
            }
            //si tout les chemins ne sont pas possible
            else
            {
                Debug.LogWarning("Chemin impossible");
                IsBloced = false; // Annuler le blocage
                nextTile = tempTile; // remettre le nextTile
                GameManager.Instance.UpdateFlowFieldAround(position); //update le flowfield
            }
        }
        //si la tuille est bloquer et en mode supression
        else if (IsBloced && GameManager.Instance.deleteTower)
        {
            // Trouver la tour la plus proche sans depasser une tuile de distance et la supprimer
            var nearbyTower = Tower.allTourel.FirstOrDefault(tower =>
                Vector3.Distance(this.transform.position, tower.transform.position) < 1);

            //si il y a une toure suprimer
            if (nearbyTower != null)
            {
                IsBloced = false;

                // Supprimer la tour
                nearbyTower.OnRevome();
                //update le flowField
                GameManager.Instance.UpdateFlowFieldAround(position);
            }
        }
    }

    private bool CanPlace()
    {
        //si la tuille a une de ces variable true 
        //cette tuille est imposible a mettre une tour
        if (IsBloced || IsEnd || IsSpawn)
        {
            return false;
        }
        else
        { return true; }
    }

    public bool IsPathAvailable(GameTiles[,] gameTile)
    {
        // Trouver la tuile de départ (Spawn)
        List<GameTiles> startTile = new List<GameTiles>();
        GameTiles endTile = null;
        int numOfPathFind = 0;

        //passe atraver toute les tuile pour trouver la tuille de fin
        //et les tuille de spawn
        foreach (GameTiles tile in gameTile)
        {
            if (tile.IsSpawn)
            { startTile.Add(tile); }
            if (tile.IsEnd)
            { endTile = tile; }
        }

        //si aucun spawn ou fin trouver envoye une erreure
        if (startTile == null || endTile == null)
        {
            Debug.LogError("Aucune tuile de départ ou de fin définie !");
            return false;
        }

        // Parcourir le Flow Field pour voir si on peut atteindre l'arrivée
        foreach (GameTiles start in startTile)
        {
            GameTiles currentTile = start; // premiere tuile a cherher
            HashSet<GameTiles> visited = new HashSet<GameTiles>(); // Évite les boucles infinies

            //parcour toute les tuile jusqua ce que la tuille soit null ou qu'elle est deja parcourue 
            while (currentTile != null && !visited.Contains(currentTile))
            {
                //ajoute cette tuile au set de tuille visiter
                visited.Add(currentTile);

                //si la tuille actuel et la tuille de fin finir
                if (currentTile == endTile)
                {
                    Debug.Log("rendu a la fin");
                    numOfPathFind++; //ajoute le nomdre de chemin trouver
                    break; //met fin a la recherche
                }

                //passe a la prochaine tuille
                currentTile = currentTile.nextTile; // Passer à la tuile suivante
            }
        }

        Debug.Log($"Nombre de chemin trouver : {numOfPathFind} sur {startTile.Count} spawn");

        //si a trouver le meme nombre de chemin possible au nombre de spawn c'est bon les chemin son valide
        if (numOfPathFind == startTile.Count)
        {
            return true;
        }
        else
        {
            return false;
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
        if (IsBloced)
        {
            cost = float.MaxValue;
        }
        else
        {
            cost = 10000;
        }
    }

    public void SetTileRender()
    {
        spriteSpawn.enabled = IsSpawn;
        spriteEnd.enabled = IsEnd;
        SlowingRenderer.enabled = IsSlowing;
        WallRenderer.enabled = IsBloced;
        DamagingRenderer.enabled = IsDamaging;
    }

}
