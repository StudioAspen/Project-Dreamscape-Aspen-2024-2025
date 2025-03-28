using DG.Tweening.Core.Easing;
using System;
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
    private GameManager gameManager;
    private NavMeshSurface navMeshSurface; // nav mesh surface for pathfinding

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
    [field: SerializeField] public float LandScale { get; private set; } = 30f;

    [Header("Land Position Selection")]
    [SerializeField] private Transform ghostLandTransform;

    [field: Header("Biome Selection")]
    [field: SerializeField] public BiomeDatabaseSO BiomeDatabase { get; private set; }
    private Biome currentBiomeSelection = Biome.DREAM;

    [field: Header("Possible Elites")]
    [field: SerializeField] public List<EliteVariantStatusEffectSO> EliteVariantStatusEffects { get; private set; } = new List<EliteVariantStatusEffectSO>();

    private void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        // Spawn a 2x2 Dream at the start
        currentBiomeSelection = Biome.DREAM;
        SpawnLand(Vector2Int.zero, BiomeDatabase.DefaultLandPrefab);
        SpawnLand(new Vector2Int(1, 0), BiomeDatabase.DefaultLandPrefab);
        SpawnLand(new Vector2Int(0, 1), BiomeDatabase.DefaultLandPrefab);
        SpawnLand(new Vector2Int(1, 1), BiomeDatabase.DefaultLandPrefab);

        BuildNavMesh();

        DisableGhostLand();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ToggleLandLevelStyle(!LandLevelStyleIsSimple);
        }
    }

    public bool LandLevelStyleIsSimple { get; private set; } = false;
    // Debug Cheat
    public void ToggleLandLevelStyle(bool isSimple)
    {
        LandLevelStyleIsSimple = isSimple;

        if (isSimple)
        {
            DisableLandLevelTexts();
            // Sort the lands by level in descending order
            List<LandManager> sortedLands = SpawnedLands.Values.OrderByDescending(land => land.Level).ToList();
            // Add the top 3 lands to the result list
            for (int i = 0; i < 3 && i < sortedLands.Count; i++)
            {
                sortedLands[i].EnableLevelText();
            }
        }
        else
        {
            EnableLandLevelTexts();
        }
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
    /// Retrieves the LandManager object based on the given world position.
    /// Returns null if no land exists at the specified position.
    /// </summary>
    /// <param name="worldPosition">The world position of the land.</param>
    /// <returns>The LandManager object at the specified grid position.</returns>
    public LandManager GetLandByWorldPosition(Vector3 worldPosition)
    {
        Vector2Int gridPosition = GetGridPosition(worldPosition);

        return GetLandByGridPosition(gridPosition);
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

    public List<LandManager> GetLandsWithManhattanDistance(Vector2Int gridPosition, int endLayer = 1, bool checkLayer0 = true) 
    {
      // If the grid position has a Land, initialize the list with that Land as the first element. Otherwise, initialize the list as empty.
      List<LandManager> lands = TryGetLandByGridPosition(gridPosition, out LandManager landZero) && checkLayer0 ? new List<LandManager>{ landZero } : new List<LandManager>();

      // search layer 1 -> endLayer for lands
      for (int layer = 1; layer <= endLayer; layer++)
      {
        for (int xOffset = -layer; xOffset <= layer; xOffset++)
        {
          int absX = Mathf.Abs(xOffset);
          int remainingSteps = layer - absX;

          if (remainingSteps > 0)
          {
            // Check the top side of the layer for a land
            if (TryGetLandByGridPosition(new Vector2Int(gridPosition.x + xOffset, gridPosition.y + remainingSteps), out LandManager topLand))
              lands.Add(topLand);

            // Check the top side of the layer for a land
            if (TryGetLandByGridPosition(new Vector2Int(gridPosition.x + xOffset, gridPosition.y - remainingSteps), out LandManager botLand))
              lands.Add(botLand);
          } else 
          {
            // Check the rightmost coordinate of the last layer for a land
            if (TryGetLandByGridPosition(new Vector2Int(gridPosition.x + xOffset, gridPosition.y), out LandManager lastLand))
              lands.Add(lastLand);
          }
        }
      }

      return lands;
    }

    public List<LandManager> GetLandsWithManhattanDistance(Vector3 worldPosition, int endLayer = 1) 
    {
      Vector2Int gridPosition = GetGridPosition(worldPosition);
      return GetLandsWithManhattanDistance(gridPosition, endLayer);
    }

    public LandManager GetFarthestLand(Vector2Int originPosition)
    {
      Vector2Int targetPosition = Vector2Int.zero;
      float farthestDistance = 0f;

      foreach (KeyValuePair<Vector2Int, LandManager> land in SpawnedLands)
      {
        Vector2Int gridPosition = land.Key;
        LandManager spawnedLand = land.Value;

        // Skip the origin position
        if (gridPosition == originPosition) continue;

        // Calculate the manhattan distance between the two grid positions
        float distance = Math.Abs(gridPosition.x - originPosition.x) + Math.Abs(gridPosition.y - originPosition.y);

        if (distance > farthestDistance)
        {
          farthestDistance = distance;
          targetPosition = gridPosition;
        }
      }

      return SpawnedLands[targetPosition];
    }

    /// <summary>
    /// Retrieves the grid position based on the given world position.
    /// </summary>
    /// <param name="worldPos">The world position.</param>
    /// <returns>The grid position.</returns>
    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        Vector3 floatGridPosition = worldPos / LandScale;

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
        return new Vector3(LandScale * gridPosition.x, height, LandScale * gridPosition.y);
    }

    /// <summary>
    /// Retrieves a random LandManager object from the list of spawned lands.
    /// </summary>
    /// <returns>A random LandManager object.</returns>
    public LandManager GetRandomLand()
    {
        int randomIndex = UnityEngine.Random.Range(0, SpawnedLands.Count);
        return SpawnedLands.Values.ElementAt(randomIndex);
    }

    /// <summary>
    /// Distributes random weights to all LandManager objects based on their land levels, then randomly selects one.
    /// </summary>
    /// <returns>A random LandManager object</returns>
    public LandManager GetRandomLandByWeight()
    {
      float random = UnityEngine.Random.Range(0f, SpawnedLands.Values.Sum(land => land.Weight));
      float current = 0;

      foreach (LandManager land in SpawnedLands.Values)
      {
        current += land.Weight;
        if (random <= current) return land;
      }
      
      return SpawnedLands.Values.First();
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
    private LandManager SpawnLand(Vector2Int gridPosition)
    {
        LandManager landPrefabToUse = null;
        if (BiomeDatabase.BiomesDictionary[currentBiomeSelection].PossibleLands.Count == 0)
        {
            Debug.LogWarning($"No possible lands for biome {currentBiomeSelection}. Using default land prefab.");
            landPrefabToUse = BiomeDatabase.DefaultLandPrefab;
        }
        else
        {
            landPrefabToUse = BiomeDatabase.BiomesDictionary[currentBiomeSelection].PossibleLands[UnityEngine.Random.Range(0, BiomeDatabase.BiomesDictionary[currentBiomeSelection].PossibleLands.Count)];
        }

        return SpawnLand(gridPosition, landPrefabToUse);
    }

    /// <summary>
    /// Spawns a new land at the given grid position with a specific land prefab.
    /// </summary>
    /// <param name="gridPosition">The grid position of the land.</param>
    /// <param name="landPrefab">The land prefab to spawn.</param>
    private LandManager SpawnLand(Vector2Int gridPosition, LandManager landPrefab)
    {
        LandManager spawnedLand = Instantiate(landPrefab, new Vector3(LandScale * gridPosition.x, 0, LandScale * gridPosition.y), Quaternion.identity, transform);
        spawnedLand.Init(gridPosition, currentBiomeSelection);

        SpawnedLands.Add(gridPosition, spawnedLand);

        return spawnedLand;
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

        gameManager.ChangeState(GameState.LAND_EMPOWERMENT);
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

    /// <summary>
    /// Assigns the given biome to be spawned next and changes the game state to LAND_PLACEMENT.
    /// </summary>
    /// <param name="biome">The biome to assign.</param>
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

    /// <summary>
    /// Sets the position of the ghost land based on the given world position.
    /// Updates the material of the ghost land renderer based on the current game state.
    /// </summary>
    /// <param name="worldPos">The world position.</param>
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

        if (gameManager.CurrentState == GameState.LAND_EMPOWERMENT)
        {
            ghostLandRenderer.material = IsLandAt(gridPosition) ? greenMaterial : redMaterial;
        }
    }

    /// <summary>
    /// Retrieves the world position of the ghost land.
    /// </summary>
    /// <returns>The world position of the ghost land.</returns>
    public Vector3 GetGhostLandPosition()
    {
        return ghostLandTransform.position;
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
}
