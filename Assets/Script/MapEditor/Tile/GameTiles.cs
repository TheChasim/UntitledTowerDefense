using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameTiles : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler, IPointerClickHandler
{
    [Header("Tille Infos")]
    [SerializeField] public bool IsSelected = false;
    [SerializeField] public bool IsSpawn = false;
    [SerializeField] public bool IsEnd = false;
    [SerializeField] public bool IsBloced = false;
    [SerializeField] public bool IsSlowing = false;
    [SerializeField] public bool IsDamaging = false;
    [SerializeField] public Element damegeType;

    [Header("Cout de deplacement")]
    [SerializeField] public float normalCost = 1;
    [SerializeField] public float damageCost = 3;
    [SerializeField] public float slowingCost = 1.5f;
    [SerializeField] public float cost;
    [SerializeField] public float DamageAmout = 0.5f;
    [SerializeField] public float SlowingAmout = 2f;

    [Space]
    [Header("General Info")]
    [SerializeField] public Vector3 worldPosition;
    [SerializeField] public int gridX, gridY;
    [Space]

    [Header("Direction")]
    //[SerializeField] internal Vector3 flowDirection = Vector3.zero;
    [SerializeField] public GameTiles nextTile;
    private LineRenderer lineRenderer; // Affichage du Flow Field
    [Space]

    [Header("Sprite Setting")]
    public SpriteRenderer renderer;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer SelectedRenderer;
    public SpriteRenderer spriteSpawn;
    public SpriteRenderer spriteEnd;
    public SpriteRenderer SlowingRenderer;
    public SpriteRenderer WallRenderer;
    public SpriteRenderer DamagingRenderer;
    private Color originalColor;
    //public GameObject Object3D;

    [Header("Tile set Layer")]
    //pour le base
    public TileSet baseLayer;//0-base layer, 1-spawn, 2-end, 3-slowing, 4-damaging, 5-wall
    public GroupTileSet natureLayer; //0- grass
    public GroupTileSet terrainLayer; //0- grass
    public GroupTileSet DecorationLayer;
    public GroupObjectTileSet Object3DLayer;
    public GroupObjectTileSet ModuleLayer;
    [Header("Tile set")]
    public TileSet grass;
    public TileSet water;

    [Header("Tile Set layer Renderer")]
    public SpriteRenderer natureRenderer;
    public SpriteRenderer terrainRenderer;
    public SpriteRenderer decorationRenderer;
    public GameObject Object3DSet;
    public GameObject ModuleSet;

    public int X { get; internal set; }
    public int Y { get; internal set; }

    internal void SetValue(Vector3 newWorldPos, int newX, int newY)
    {
        worldPosition = newWorldPos;
        gridX = newX;
        gridY = newY;
    }
    internal void SetComponent(int X, int Y)
    {
        gridX = X;
        gridY = Y;
    }
    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //originalColor = spriteRenderer.color;
        //GetComponent<SpriteRenderer>().sprite = spriteRenderer[UnityEngine.Random.Range(0, spriteRenderer.Count())];

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

    public void TurnBlank()
    {
        renderer.sprite = baseLayer.tiles[0];
        IsSpawn = false;
        IsBloced = false;
        IsEnd = false;
        IsDamaging = false;
    }

    public void TurnSpawn()
    {
        IsSpawn = !IsSpawn;
        spriteSpawn.enabled = IsSpawn;
    }

    public void TurnEnd()
    {
        IsEnd = !IsEnd;
        spriteEnd.enabled = IsEnd;
    }

    public void TurnGrey(float alphaValue)
    {
        Color currentColor = originalColor;
        Color newColor = new Color(currentColor.grayscale, currentColor.grayscale, currentColor.grayscale, 0.5f);
        //spriteRenderer.color = newColor;
    }

    public void TurnBloced()
    {
        //IsBloced = !IsBloced;
        IsBloced = true; 
        //WallRenderer.enabled = IsBloced;
        renderer.sprite = baseLayer.tiles[5];
    }

    public void TurnSlow()
    {
        IsSlowing = !IsSlowing;
        //SlowingRenderer.enabled = IsSlowing;
        renderer.sprite = baseLayer.tiles[3];
    }

    public void TurnDamaging()
    {
        IsDamaging = !IsDamaging;
        //DamagingRenderer.enabled = IsDamaging;
        renderer.sprite = baseLayer.tiles[4];
    }

    public void SetPathColor(bool isPath)
    {
        Color transparentOrange = new Color(1, 0.375f, 0, 0.5f);

        //spriteRenderer.color = isPath ? transparentOrange : originalColor;
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

    internal void RemoveObject(int layer) // 0 pour object 1 pour Module
    {
        GameObject obj = null;
        bool find = false;

        foreach (Transform child in this.transform)
        {

            switch (layer)
            {
                case 0:
                    if (Object3DSet != null)
                    {
                        if (child.gameObject.GetComponent<SpriteRenderer>().sprite == Object3DSet.GetComponent<SpriteRenderer>().sprite)
                        {
                            //DestroyImmediate(child.gameObject);

                            if (Object3DSet.CompareTag("obstacle"))
                            {
                                TurnBlank();
                            }

                            Object3DSet = null;
                            obj = child.gameObject;
                            find = true;
                            break;
                        }
                    }
                    break;
                case 1:
                    if (ModuleSet != null)
                    {
                        if (child.gameObject.layer == 3)
                        {
                            //DestroyImmediate(child.gameObject);

                            if (ModuleSet.CompareTag("obstacle"))
                            {
                                TurnBlank();
                            }

                            ModuleSet = null;
                            obj = child.gameObject;
                            find = true;
                            break;
                        }
                    }
                    break;

            }
           
        }


        if (find)
        {
            DestroyImmediate(obj);
            Object3DSet = null;
        }
    }

    public void SetTileRender()
    {
        //spriteSpawn.enabled = IsSpawn;
        //spriteEnd.enabled = IsEnd;
        //SlowingRenderer.enabled = IsSlowing;
        //WallRenderer.enabled = IsBloced;
        //DamagingRenderer.enabled = IsDamaging;

        if (IsBloced)
        { renderer.sprite = baseLayer.tiles[5]; }
        else if (IsDamaging)
        { renderer.sprite = baseLayer.tiles[4]; }
        else if (IsSlowing)
        { renderer.sprite = baseLayer.tiles[3]; }
        else if (IsEnd)
        { 
            spriteEnd.enabled = true;
            spriteSpawn.enabled = false;
        }
        else if(IsSpawn)
        {
            spriteEnd.enabled = false;
            spriteSpawn.enabled = true;
        }
        else
        { renderer.sprite = baseLayer.tiles[0]; }

        if(!IsEnd && !IsSpawn)
        {
            spriteEnd.enabled = false;
            spriteSpawn.enabled = false;
        }
    }

    public void SetTileRenderNature(int natureLayerIndex, bool autofil, int spriteIndex)
    {
        if (autofil)
        {
            if (spriteIndex == -1)
            {
                natureRenderer.sprite = null;
            }
            else
            {
                natureRenderer.sprite = this.natureLayer.groupSet[natureLayerIndex].tiles[Random.Range(0, this.natureLayer.groupSet[natureLayerIndex].tiles.Length)];
            }
        }
        else
        {
            if (spriteIndex == -1)
            {
                natureRenderer.sprite = null;
            }
            else
            {
                natureRenderer.sprite = this.natureLayer.groupSet[natureLayerIndex].tiles[spriteIndex];
            }
        }
    }

    public void SetTileRenderTerrain(int natureLayerIndex, bool autofil, int spriteIndex)
    {
        if (autofil)
        {
            if (spriteIndex == -1)
            {
                terrainRenderer.sprite = null;
            }
            else
            {
                terrainRenderer.sprite = this.terrainLayer.groupSet[natureLayerIndex].tiles[Random.Range(0, this.terrainLayer.groupSet[natureLayerIndex].tiles.Length)];
            }
        }
        else
        {
            if (spriteIndex == -1)
            {
                terrainRenderer.sprite = null;
            }
            else
            {
                terrainRenderer.sprite = this.terrainLayer.groupSet[natureLayerIndex].tiles[spriteIndex];
            }
        }
    }

    public void SetTileRenderDecoration(int decorationLayerIndex, bool autofil, int spriteIndex)
    {
        if (autofil)
        {
            if (spriteIndex == -1)
            {
                decorationRenderer.sprite = null;
            }
            else
            {
                decorationRenderer.sprite = this.DecorationLayer.groupSet[decorationLayerIndex].tiles[Random.Range(0, this.DecorationLayer.groupSet[decorationLayerIndex].tiles.Length)];
            }
        }
        else
        {
            if (spriteIndex == -1)
            {
                decorationRenderer.sprite = null;
            }
            else
            {
                decorationRenderer.sprite = this.DecorationLayer.groupSet[decorationLayerIndex].tiles[spriteIndex];
            }
        }
    }

    public void SetObjectTileRenderTerrain(int Object3DLayerIndex, bool autofil, int spriteIndex)
    {
        if (autofil)
        {
            if (spriteIndex == -1)
            {
                RemoveObject(0);// un set 0 pour object set 1 pour module
            }
            else
            {
                if (spriteIndex == -1)
                {
                    RemoveObject(0);
                }
                else
                {
                    if (Object3DSet != null)
                    {
                        RemoveObject(0);
                    }

                    Object3DSet = this.Object3DLayer.objectTileSet[Object3DLayerIndex].objects[Random.Range(0, this.Object3DLayer.objectTileSet[Object3DLayerIndex].objects.Length)];

                    //var inst = (GameObject)PrefabUtility.InstantiatePrefab(Object3DSet);
                    var inst = Instantiate(Object3DSet);
                    inst.transform.position = new Vector3(transform.position.x, inst.transform.position.y, transform.position.z);
                    inst.transform.parent = transform;

                    if (Object3DSet.CompareTag("obstacle"))
                    {
                        TurnBloced();
                    }

                }
            }
        }
        else
        {
            if (spriteIndex == -1)
            {
                RemoveObject(0);
            }
            else
            {
                if (Object3DSet != null)
                {
                    RemoveObject(0);
                }

                Object3DSet = this.Object3DLayer.objectTileSet[Object3DLayerIndex].objects[spriteIndex];

                //var inst = (GameObject)PrefabUtility.InstantiatePrefab(Object3DSet);
                var inst = Instantiate(Object3DSet);
                inst.transform.position = new Vector3(transform.position.x, inst.transform.position.y, transform.position.z);
                inst.transform.parent = transform;

                if (Object3DSet.CompareTag("obstacle"))
                {
                    TurnBloced();
                }

            }
        }
    }

    public void SetModuleTileRenderTerrain(int moduleLayerIndex, bool autofil, int spriteIndex)
    {
        if (autofil)
        {
            if (spriteIndex == -1)
            {
                RemoveObject(1); // un set 0 pour object set 1 pour module
            }
            else
            {
                if (spriteIndex == -1)
                {
                    RemoveObject(1);
                }
                else
                {
                    if (ModuleSet != null)
                    {
                        RemoveObject(1);
                    }

                    ModuleSet = this.ModuleLayer.objectTileSet[moduleLayerIndex].objects[Random.Range(0, this.ModuleLayer.objectTileSet[moduleLayerIndex].objects.Length)];

                    //var inst = (GameObject)PrefabUtility.InstantiatePrefab(ModuleSet);
                    var inst = Instantiate(ModuleSet);
                    inst.transform.position = new Vector3(transform.position.x, inst.transform.position.y, transform.position.z);
                    inst.transform.parent = transform;

                    if (ModuleSet.CompareTag("obstacle"))
                    {
                        TurnBloced();
                    }

                }
            }
        }
        else
        {
            if (spriteIndex == -1)
            {
                RemoveObject(1);
            }
            else
            {
                if (ModuleSet != null)
                {
                    RemoveObject(1);
                }

                ModuleSet = this.ModuleLayer.objectTileSet[moduleLayerIndex].objects[spriteIndex];

                //var inst = (GameObject)PrefabUtility.InstantiatePrefab(ModuleSet);
                var inst = Instantiate(ModuleSet);
                inst.transform.position = new Vector3(transform.position.x, inst.transform.position.y, transform.position.z);
                inst.transform.parent = transform;

                if (ModuleSet.CompareTag("obstacle"))
                {
                    TurnBloced();
                }

            }
        }
    }
}
