using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    #region Variable Initializations
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private NavMeshSurface navMeshSurface;

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
    public List<LandManager> SpawnedLands { get; private set; } = new List<LandManager>();
    private List<LandBorder> bordersList = new List<LandBorder>();
    private int activeLandCount;

    [Header("Land Position Selection")]
    [SerializeField] private LandManager landToSpawnPrefab;
    [SerializeField] private Transform ghostLandTransform;
    [SerializeField] private Material redTransparentMaterial;
    [SerializeField] private Material greenTransparentMaterial;

    [field: Header("Biome Selection")]
    private Biome currentBiomeSelection = Biome.DREAM;

    [field: Header("Event Selection")]
    public WorldEvent currentEventSelection = WorldEvent.SURVIVAL;

    [field: Header("Progression")]
    public int EmpowerTokens { get; private set; }
    public int WeakenTokens { get; private set; }

    private int activePrioritiesCount = 0;
    #endregion

    #region On Start
    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake() { }

    void Start()
    {
        SpawnedLands.Add(GetComponentInChildren<LandManager>());
        BuildNavMesh();
        activeLandCount = 1;
        DisableGhostLand();
    }
    #endregion

    void Update() { }

    #region Grid Functions
    // Searches for and returns the LandManager at the specified grid position (x, y). If no land exists at that position, returns null.
    public LandManager GetLand(int x, int y)
    {
        foreach (LandManager land in SpawnedLands)
        {
            if (land.GridPosition.x == x && land.GridPosition.y == y) return land;
        }
        return null;
    }

    // Converts a world position (Vector3) to a grid position (Vector2Int) by scaling the world position according to the land's size.
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        float landScale = landToSpawnPrefab.transform.localScale.x;
        Vector3 floatGridPosition = worldPos / landScale;
        return new Vector2Int(Mathf.RoundToInt(floatGridPosition.x), Mathf.RoundToInt(floatGridPosition.z));
    }

    // Converts a grid position (Vector2Int) to a world position (Vector3) at a specified height, taking into account the land's scale.
    public Vector3 GetLandPosition(Vector2Int gridPosition, float height)
    {
        float landScale = landToSpawnPrefab.transform.localScale.x;
        return new Vector3(landScale * gridPosition.x, height, landScale * gridPosition.y);
    }
    #endregion

    #region Intermediary Processes
    // Checks if a new land can be spawned at the given grid position by verifying if the position matches any existing border.
    private bool CanNewLandSpawnAt(Vector2Int gridPos)
    {
        bool isPositionValid = false;
        for (int i = 0; i < bordersList.Count; i++)
        {
            if (bordersList[i].WorldBorderPosition == gridPos)
            {
                isPositionValid = true;
                break;
            }
        }
        return isPositionValid;
    }

    // Spawns a new land at the specified grid coordinates (x, y) and adds it to the list of spawned lands.
    private void SpawnLand(int x, int y)
    {
        float landScale = landToSpawnPrefab.transform.localScale.x;
        LandManager spawnedLand = Instantiate(landToSpawnPrefab, new Vector3(landScale * x, -5f, landScale * y), Quaternion.identity, transform);
        spawnedLand.Init(x, y);
        SpawnedLands.Add(spawnedLand);
    }

    // Tries to spawn a new land at the position of the ghost land, if valid, otherwise logs an error and prevents spawning.
    public void TrySpawnLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);
        if (!CanNewLandSpawnAt(spawnPosition))
        {
            Debug.Log("Can't spawn new land at this ghost position");
            return;
        }
        DisableGhostLand();
        SpawnLand(spawnPosition.x, spawnPosition.y);
        RestockTokens(SpawnedLands.Count);
        gameManager.ChangeState(GameState.LAND_EMPOWERMENT);
    }

    // Resets land level differences and changes the game state to event selection.
    public void ContinueToEventSelection()
    {
        ResetLandLevelDifferences();
        gameManager.ChangeState(GameState.EVENT_SELECTION);
    }

    // Builds the navigation mesh for the current environment.
    public void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    // Adds a new land border to the list of borders.
    public void AddBorder(LandBorder border)
    {
        bordersList.Add(border);
    }

    // Removes connected borders by iterating through spawned lands and borders, destroying the borders that are connected to existing lands.
    public void RemoveConnectedBorders()
    {
        foreach (LandManager land in new List<LandManager>(SpawnedLands))
        {
            foreach (LandBorder border in new List<LandBorder>(bordersList))
            {
                if (land.GridPosition == border.WorldBorderPosition)
                {
                    Destroy(border.gameObject);
                    bordersList.Remove(border);
                }
            }
        }
    }
    #endregion

    #region EOW Conditions
    // Prepares the game for the next wave based on the current event selection. 
    // It resets enemy spawners for all lands and handles specific logic for different event types (e.g., SURVIVAL, PRIORITIES, etc.).
    public void PrepareForNextWave()
    {
        if (currentEventSelection == WorldEvent.SURVIVAL)
        {
            activeLandCount = SpawnedLands.Count;
            foreach (LandManager land in SpawnedLands)
            {
                land.EnemySpawner.WaveReset();
            }
        }

        if (currentEventSelection == WorldEvent.PRIORITIES)
        {
            if (SpawnedLands.Count <= 3)
            {
                foreach (LandManager land in SpawnedLands) //WAIT ACTUALLY, IF THERE ARE LESS THAN 4 LANDS, THEN THE WAVE FUNCTIONS THE EXACT SAME AS THE SURVIVAL WAVE, SO THIS CAN ALL BE SCRAPPED
                {
                    activePrioritiesCount += 1;
                    land.EnemySpawner.IsPriority = true;
                    land.EnemySpawner.WaveReset();
                }
                Debug.Log(activePrioritiesCount);
            }
            else //There are at least 4 lands present
            {
                activePrioritiesCount = 3;

                LandManager highestLevelLand = null;
                LandManager secondHighestLevelLand = null;
                LandManager thirdHighestLevelLand = null;

                foreach (LandManager land in SpawnedLands)
                {
                    if (highestLevelLand == null || land.Level > highestLevelLand.Level)
                    {
                        thirdHighestLevelLand = secondHighestLevelLand;
                        secondHighestLevelLand = highestLevelLand;
                        highestLevelLand = land;
                    }
                    else if (secondHighestLevelLand == null || land.Level > highestLevelLand.Level)
                    {
                        thirdHighestLevelLand = secondHighestLevelLand;
                        secondHighestLevelLand = land;
                    }
                    else if (thirdHighestLevelLand == null || land.Level > thirdHighestLevelLand.Level)
                    {
                        thirdHighestLevelLand = land;
                    }
                }

                highestLevelLand.EnemySpawner.IsPriority = true;
                secondHighestLevelLand.EnemySpawner.IsPriority = true;
                thirdHighestLevelLand.EnemySpawner.IsPriority = true;

                foreach (LandManager land in SpawnedLands)
                {
                    land.EnemySpawner.WaveReset();
                }

            }
        }
        //Koi's quick find marker
        //will need to only be active as an option when spawnedlands < 36 (1/4)
        //todo: implement a matrix or array so that it's actually possible to search for
        //3x3 grids rather than searching nearby tiles
        if (currentEventSelection == WorldEvent.ZONES)
        {
            //pick a 3x3
            //spawn enemies in that area
        }

        if (currentEventSelection == WorldEvent.ESCORT ||
            currentEventSelection == WorldEvent.DEFEND ||
            currentEventSelection == WorldEvent.VISITALL)
        {
            activeLandCount = SpawnedLands.Count;
            foreach (LandManager land in SpawnedLands)
            {
                land.EnemySpawner.WaveReset();
            }
        }
    }

    // Decrements the active land count and transitions the game to biome selection if all lands are processed.
    public void DecrementActiveLandCount()
    {
        activeLandCount--;
        if (activeLandCount == 0)
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
        }
    }

    // Decrements the active priorities count and despawns all non-priority enemies if there are no active priorities left, 
    // then transitions the game to biome selection.
    public void DecrementActivePrioritiesCount()
    {
        Debug.Log("Decremented!!!!!");
        activePrioritiesCount--;
        if (activePrioritiesCount == 0)
        {
            foreach (LandManager land in SpawnedLands)
            {
                if (!land.EnemySpawner.IsPriority)
                {
                    land.EnemySpawner.DespawnAllEnemies();
                }
                else
                {
                    land.EnemySpawner.IsPriority = false;
                }
            }
            gameManager.ChangeState(GameState.BIOME_SELECTION);
        }
    }
    #endregion

    #region Player Selection
    // Assigns the next biome to be used for land placement and changes the game state to LAND_PLACEMENT.
    public void AssignBiomeToSpawnNext(Biome biome)
    {
        currentBiomeSelection = biome;
        gameManager.ChangeState(GameState.LAND_PLACEMENT);
    }

    // Assigns the next world event and prepares for the next wave of enemies, then changes the game state to PLAYING.
    public void AssignNextEvent(WorldEvent worldEvent)
    {
        currentEventSelection = worldEvent;
        PrepareForNextWave();
        gameManager.ChangeState(GameState.PLAYING);
    }
    #endregion

    #region Ghost Land Functions
    // Enables the ghost land object, making it visible in the game world.
    public void EnableGhostLand()
    {
        ghostLandTransform.gameObject.SetActive(true);
    }

    // Disables the ghost land object, making it invisible in the game world.
    public void DisableGhostLand()
    {
        ghostLandTransform.gameObject.SetActive(false);
    }

    // Sets the position of the ghost land based on the provided world position, adjusting its appearance depending on whether land can be placed there.
    public void SetGhostLandPosition(Vector3 worldPos)
    {
        Vector2Int gridPosition = GetGridPosition(worldPos);
        ghostLandTransform.position = GetLandPosition(gridPosition, ghostLandTransform.position.y);

        if (gameManager.CurrentState == GameState.LAND_PLACEMENT)
        {
            ghostLandTransform.GetComponent<MeshRenderer>().material = CanNewLandSpawnAt(gridPosition) ? greenTransparentMaterial : redTransparentMaterial;
        }

        if (gameManager.CurrentState == GameState.LAND_EMPOWERMENT)
        {
            ghostLandTransform.GetComponent<MeshRenderer>().material = GetLand(gridPosition.x, gridPosition.y) != null ? greenTransparentMaterial : redTransparentMaterial;
        }
    }

    // Enables the display of level text on all spawned lands.
    public void EnableLandLevelTexts()
    {
        foreach (LandManager land in SpawnedLands)
        {
            land.EnableLevelText();
        }
    }

    // Disables the display of level text on all spawned lands.
    public void DisableLandLevelTexts()
    {
        foreach (LandManager land in SpawnedLands)
        {
            land.DisableLevelText();
        }
    }
    #endregion

    #region Progression Functions
    // Restocks the empower and weaken tokens based on the current land count. 
    // Empower tokens are rounded up, and weaken tokens are rounded down.
    private void RestockTokens(int landCount)
    {
        EmpowerTokens = Mathf.CeilToInt((landCount - 1) / 2f);
        WeakenTokens = Mathf.FloorToInt((landCount - 1) / 2f);
    }

    // Attempts to empower the land at the ghost position, checking if the land can be empowered 
    // and if there are enough empower tokens. Increases the land's level and adjusts the token counts accordingly.
    public void TryEmpowerLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);
        LandManager hoveredLand = GetLand(spawnPosition.x, spawnPosition.y);

        if (hoveredLand == null)
        {
            Debug.Log("Can't empower at this land");
            return;
        }

        if (hoveredLand.LevelDifference >= 0 && EmpowerTokens <= 0)
        {
            Debug.Log("Not enough empower tokens");
            return;
        }

        if (hoveredLand.LevelDifference < 0)
        {
            WeakenTokens++;
            EmpowerTokens++;
        }

        hoveredLand.AddLevel(1);
        EmpowerTokens--;
    }

    // Attempts to weaken the land at the ghost position, checking if the land can be weakened 
    // and if there are enough weaken tokens. Decreases the land's level and adjusts the token counts accordingly.
    public void TryWeakenLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);
        LandManager hoveredLand = GetLand(spawnPosition.x, spawnPosition.y);

        if (hoveredLand == null)
        {
            Debug.Log("Can't weaken at this land");
            return;
        }

        if (hoveredLand.LevelDifference <= 0 && WeakenTokens <= 0)
        {
            Debug.Log("Not enough weaken tokens");
            return;
        }

        if (hoveredLand.LevelDifference > 0)
        {
            EmpowerTokens++;
            WeakenTokens++;
        }

        hoveredLand.AddLevel(-1);
        WeakenTokens--;
    }

    // Returns true if there are no tokens left to empower or weaken, indicating that progression can proceed.
    public bool CanProceedFromEmpowerment()
    {
        return EmpowerTokens + WeakenTokens <= 0;
    }

    // Resets the level differences of all spawned lands, effectively clearing their level status.
    private void ResetLandLevelDifferences()
    {
        foreach (LandManager land in SpawnedLands)
        {
            land.ResetLevelDifference();
        }
    }

    // Resets the progression changes for all spawned lands by adjusting token counts 
    // based on level differences and undoing any level changes made.
    public void ResetProgressionChanges()
    {
        foreach (LandManager land in SpawnedLands)
        {
            if (land.LevelDifference > 0) EmpowerTokens += Mathf.Abs(land.LevelDifference);
            if (land.LevelDifference < 0) WeakenTokens += Mathf.Abs(land.LevelDifference);

            land.UndoLevelChanges();
        }
    }
    #endregion
}

#region ENUMERATORS
// Enumeration for different biomes in the game world.
public enum Biome
{
    DREAM,
    FIRE,
    FOOD,
    BIOME3
}

// Enumeration for different events that can occur in the game world.
public enum WorldEvent
{
    SURVIVAL,
    ZONES,
    PRIORITIES,
    ESCORT,
    VISITALL,
    DEFEND
}
#endregion