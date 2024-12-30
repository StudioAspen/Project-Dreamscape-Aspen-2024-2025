using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private NavMeshSurface navMeshSurface; // nav mesh surface for pathfinding

    /* Pseudo Grid System Explanation:
     The grid system starts with a land piece placed at position (0, 0). 
     New lands are placed by calling the SpawnLand function, which multiplies the given (x, y) position by the scale of 
     the land prefab to determine the world position. This ensures that lands are placed at the correct distances from 
     each other based on their size.
     Lands can only be placed adjacent to preexisting lands. The system tracks the borders of existing lands, which 
     represent the valid positions where new land can be placed. Initially, with just one land, there are 4 possible 
     borders (up, down, left, and right). After a new land is placed, the system updates the borders to reflect the 
     newly available positions for further land placement.
    */
    [field: Header("Pseudo Grid")]
    public Dictionary<Vector2Int, LandManager> SpawnedLands { get; private set; } = new Dictionary<Vector2Int, LandManager>(); // a list of all currently spawned lands
    public Dictionary<Vector2Int, List<LandBorder>> Borders { get; private set; } = new Dictionary<Vector2Int, List<LandBorder>>(); // a list of all currently available borders
    private float landScale;

    [Header("Land Position Selection")]
    [SerializeField] private LandManager landToSpawnPrefab;
    [SerializeField] private Transform ghostLandTransform;

    [field: Header("Biome Selection")]
    private Biome currentBiomeSelection = Biome.DREAM;

    [field: Header("Progression")]
    public int EmpowerTokens { get; private set; }
    public int WeakenTokens { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        
    }

    void Start()
    {
        SpawnedLands.Add(new Vector2Int(0, 0), GetComponentInChildren<LandManager>());

        BuildNavMesh();

        DisableGhostLand();

        landScale = landToSpawnPrefab.transform.localScale.x;
    }

    void Update()
    {
        
    }

    #region Grid Functions
    /// <summary>
    /// Retrieves the LandManager object based on the given grid position.
    /// Returns null if no land exists at the specified position.
    /// </summary>
    /// <param name="gridPosition">The grid position of the land.</param>
    /// <returns>The LandManager object at the specified grid position.</returns>
    public LandManager GetLandByGridPosition(Vector2Int gridPosition)
    {
        if (!SpawnedLands.ContainsKey(gridPosition))
        {
            Debug.LogWarning($"No land at {gridPosition.x}, {gridPosition.y}");
            return null;
        }

        return SpawnedLands[gridPosition];
    }

    /// <summary>
    /// Tries to retrieve the LandManager object based on the given grid position.
    /// Returns true and assigns the LandManager object to the 'land' parameter if a land exists at the specified position.
    /// Returns false and assigns null to the 'land' parameter if no land exists at the specified position.
    /// </summary>
    /// <param name="gridPosition">The grid position of the land.</param>
    /// <param name="land">The LandManager object at the specified grid position.</param>
    /// <returns>True if a land exists at the specified position, false otherwise.</returns>
    public bool TryGetLandByGridPosition(Vector2Int gridPosition, out LandManager land)
    {
        if (!SpawnedLands.ContainsKey(gridPosition))
        {
            land = null;
            return false;
        }

        land = SpawnedLands[gridPosition];
        return true;
    }

    /// <summary>
    /// Tries to retrieve the LandManager object based on the given world position.
    /// Returns true and assigns the LandManager object to the 'land' parameter if a land exists at the specified position.
    /// Returns false and assigns null to the 'land' parameter if no land exists at the specified position.
    /// </summary>
    /// <param name="worldPosition">The world position.</param>
    /// <param name="land">The LandManager object at the specified grid position.</param>
    /// <returns>True if a land exists at the specified position, false otherwise.</returns>
    public bool TryGetLandByWorldPosition(Vector3 worldPosition, out LandManager land)
    {
        Vector2Int gridPosition = GetGridPosition(worldPosition);

        return TryGetLandByGridPosition(gridPosition, out land);
    }

    /// <summary>
    /// Retrieves the grid position based on the given world position.
    /// </summary>
    /// <param name="worldPos">The world position.</param>
    /// <returns>The grid position.</returns>
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3 floatGridPosition = worldPos / landScale;

        return new Vector2Int(Mathf.RoundToInt(floatGridPosition.x), Mathf.RoundToInt(floatGridPosition.z));
    }

    /// <summary>
    /// Calculates the new world position for a new land based on the given grid position and height.
    /// </summary>
    /// <param name="gridPosition">The grid position of the land.</param>
    /// <param name="height">The height of the land.</param>
    /// <returns>The new position for the land.</returns>
    public Vector3 CalculateNewLandWorldPosition(Vector2Int gridPosition, float height)
    {
        return new Vector3(landScale * gridPosition.x, height, landScale * gridPosition.y);
    }

    /// <summary>
    /// Retrieves a random LandManager object from the list of spawned lands.
    /// </summary>
    /// <returns>A random LandManager object.</returns>
    public LandManager GetRandomLand()
    {
        int randomIndex = Random.Range(0, SpawnedLands.Count);
        return SpawnedLands.Values.ElementAt(randomIndex);
    }
    #endregion

    /// <summary>
    /// Checks if a new land can spawn at the given grid position.
    /// </summary>
    /// <param name="gridPos">The grid position to check.</param>
    /// <returns>True if a new land can spawn at the given grid position, false otherwise.</returns>
    private bool CanNewLandSpawnAt(Vector2Int gridPos)
    {
        return Borders.ContainsKey(gridPos);
    }

    /// <summary>
    /// Checks if there is a land at the given grid position.
    /// </summary>
    /// <param name="gridPos">The grid position to check.</param>
    /// <returns>True if there is a land at the given grid position, false otherwise.</returns>
    private bool IsLandAt(Vector2Int gridPos)
    {
        return SpawnedLands.ContainsKey(gridPos);
    }

    /// <summary>
    /// Spawns a new land at the given grid position.
    /// </summary>
    /// <param name="gridPosition">The grid position of the land.</param>
    private void SpawnLand(Vector2Int gridPosition)
    {
        float landScale = landToSpawnPrefab.transform.localScale.x;

        LandManager spawnedLand = Instantiate(landToSpawnPrefab, new Vector3(landScale * gridPosition.x, -5f, landScale * gridPosition.y), Quaternion.identity, transform);
        spawnedLand.Init(gridPosition);

        SpawnedLands.Add(gridPosition, spawnedLand);
    }

    /// <summary>
    /// Tries to spawn a new land at the position of the ghost land.
    /// </summary>
    public void TrySpawnLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);
        if (!CanNewLandSpawnAt(spawnPosition))
        {
            //Debug.LogWarning("Can't spawn new land at this ghost position");
            return;
        }

        DisableGhostLand();
        SpawnLand(spawnPosition);

        RestockTokens(SpawnedLands.Count);
        gameManager.ChangeState(GameState.LAND_EMPOWERMENT);
    }

    /// <summary>
    /// Continues to the event selection phase, resetting the land level differences and changing the game state.
    /// </summary>
    public void ContinueToEventSelection()
    {
        ResetLandLevelDifferences();

        gameManager.ChangeState(GameState.EVENT_SELECTION);
    }

    /// <summary>
    /// Rebuilds the navigation mesh for enemy pathfinding to recognize any new lands.
    /// </summary>
    public void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    /// <summary>
    /// Adds a new border to the list of available borders.
    /// Destroys the border if a border already exists at the specified position.
    /// </summary>
    /// <param name="newBorder">The new border to add.</param>
    public void AddBorder(LandBorder newBorder)
    {
        if (Borders.ContainsKey(newBorder.WorldBorderPosition))
        {
            Borders[newBorder.WorldBorderPosition].Add(newBorder);
            return;
        }

        Borders.Add(newBorder.WorldBorderPosition, new List<LandBorder>() { newBorder });
    }

    /// <summary>
    /// Removes the connected borders for all spawned lands.
    /// Used to cleanup.
    /// </summary>
    public void RemoveConnectedBorders()
    {
        //Debug.Log("Cleaning up connected borders");
        foreach (Vector2Int landPosition in SpawnedLands.Keys)
        {
            // If there are borders at the land position, destroy them
            if (Borders.ContainsKey(landPosition))
            {
                foreach (LandBorder border in Borders[landPosition])
                {
                    Destroy(border.gameObject);
                }
                Borders.Remove(landPosition);
            }
        }
    }

    public void AssignBiomeToSpawnNext(Biome biome)
    {
        currentBiomeSelection = biome;

        gameManager.ChangeState(GameState.LAND_PLACEMENT);
    }

    #region Ghost Land Functions
    public void EnableGhostLand()
    {
        ghostLandTransform.gameObject.SetActive(true);
    }

    public void DisableGhostLand()
    {
        ghostLandTransform.gameObject.SetActive(false);
    }

    public void SetGhostLandPosition(Vector3 worldPos)
    {
        Vector2Int gridPosition = GetGridPosition(worldPos);

        ghostLandTransform.position = CalculateNewLandWorldPosition(gridPosition, ghostLandTransform.position.y);
        Renderer ghostLandRenderer = ghostLandTransform.GetComponent<Renderer>();

        Material greenMaterial = CustomDebug.GetTransparentMaterial();
        greenMaterial.color = new Color(0, 1, 0, 0.5f);
        Material redMaterial = CustomDebug.GetTransparentMaterial();
        redMaterial.color = new Color(1, 0, 0, 0.5f);

        if (gameManager.CurrentState == GameState.LAND_PLACEMENT)
        {
            ghostLandRenderer.material = CanNewLandSpawnAt(gridPosition) ? greenMaterial : redMaterial;
        }

        if(gameManager.CurrentState == GameState.LAND_EMPOWERMENT)
        {
            ghostLandRenderer.material = IsLandAt(gridPosition) ? greenMaterial : redMaterial;
        }
    }
    #endregion

    #region Land Level Texts
    public void EnableLandLevelTexts()
    {
        foreach(LandManager land in SpawnedLands.Values)
        {
            land.EnableLevelText();
        }
    }

    public void DisableLandLevelTexts()
    {
        foreach (LandManager land in SpawnedLands.Values)
        {
            land.DisableLevelText();
        }
    }
    #endregion

    #region Progression Functions
    private void RestockTokens(int landCount)
    {
        EmpowerTokens = Mathf.CeilToInt((landCount - 1) / 2f);
        WeakenTokens = Mathf.FloorToInt((landCount - 1) / 2f);
    }

    /// <summary>
    /// Tries to empower the land at the position of the ghost land.
    /// </summary>
    public void TryEmpowerLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);

        LandManager hoveredLand = GetLandByGridPosition(spawnPosition);

        if (hoveredLand == null)
        {
            //Debug.LogWarning("Can't empower at this land");
            return;
        }

        if (hoveredLand.LevelDifference >= 0 && EmpowerTokens <= 0)
        {
            //Debug.LogWarning("Not enough empower tokens");
            return;
        }

        if (hoveredLand.LevelDifference < 0) // if already been weakened
        {
            WeakenTokens++;
            EmpowerTokens++;
        }

        hoveredLand.AddLevel(1);
        EmpowerTokens--;
    }

    /// <summary>
    /// Tries to weaken the land at the position of the ghost land.
    /// </summary>
    public void TryWeakenLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);

        LandManager hoveredLand = GetLandByGridPosition(spawnPosition);

        if (hoveredLand == null)
        {
            //Debug.LogWarning("Can't weaken at this land");
            return;
        }

        if (hoveredLand.LevelDifference <= 0 && WeakenTokens <= 0)
        {
            //Debug.LogWarning("Not enough weaken tokens");
            return;
        }

        if (hoveredLand.LevelDifference > 0) // if already been empowered
        {
            EmpowerTokens++;
            WeakenTokens++;
        }

        hoveredLand.AddLevel(-1);
        WeakenTokens--;
    }

    /// <summary>
    /// Checks if all tokens have been used.
    /// </summary>
    /// <returns>True if all tokens have been used, false otherwise.</returns>
    public bool CanProceedFromEmpowerment()
    {
        return EmpowerTokens + WeakenTokens <= 0;
    }

    /// <summary>
    /// Resets the level differences of all spawned lands.
    /// </summary>
    private void ResetLandLevelDifferences()
    {
        foreach (LandManager land in SpawnedLands.Values)
        {
            land.ResetLevelDifference();
        }
    }

    /// <summary>
    /// Refunds the progression changes made to the lands.
    /// The level difference of each land stores the changes made to the land levels during the progression phase.
    /// </summary>
    public void RefundProgressionChanges()
    {
        foreach (LandManager land in SpawnedLands.Values)
        {
            if (land.LevelDifference > 0) EmpowerTokens += Mathf.Abs(land.LevelDifference);
            if (land.LevelDifference < 0) WeakenTokens += Mathf.Abs(land.LevelDifference);

            land.UndoLevelChanges();
        }
    }
    #endregion
}

public enum Biome
{
    DREAM,
    FIRE,
    FOOD,
    BIOME3
}
