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
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private NavMeshSurface navMeshSurface; // nav mesh surface for pathfinding

    [field: Header("Pseudo Grid")]
    public List<LandManager> SpawnedLands { get; private set; } = new List<LandManager>(); // a list of all currently spawned lands
    private List<IslandBorder> bordersList = new List<IslandBorder>();
    private int activeLandCount;

    [Header("Land Position Selection")]
    [SerializeField] private LandManager islandToSpawnPrefab;
    [SerializeField] private Transform ghostLandTransform;
    [SerializeField] private Material redTransparentMaterial;
    [SerializeField] private Material greenTransparentMaterial;

    [field: Header("Biome Selection")]
    private Biome currentlySelectedBiome = Biome.DREAM;

    [field: Header("Progression")]
    [field: SerializeField] public int EmpowerTokens { get; private set; }
    [field: SerializeField] public int WeakenTokens { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        
    }

    void Start()
    {
        SpawnedLands.Add(GetComponentInChildren<LandManager>());

        BuildNavMesh();

        activeLandCount = 1;

        DisableGhostLand();
    }

    void Update()
    {
        
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        float islandScale = islandToSpawnPrefab.transform.localScale.x;

        Vector3 floatGridPosition = worldPos / islandScale;

        return new Vector2Int(Mathf.RoundToInt(floatGridPosition.x), Mathf.RoundToInt(floatGridPosition.z));
    }

    public Vector3 GetLandPosition(Vector2Int gridPosition, float height)
    {
        float islandScale = islandToSpawnPrefab.transform.localScale.x;

        return new Vector3(islandScale * gridPosition.x, height, islandScale * gridPosition.y);
    }

    public LandManager GetLand(int x, int y)
    {
        foreach(LandManager island in new List<LandManager>(SpawnedLands))
        {
            if(island.GridPosition.x == x && island.GridPosition.y == y) return island;
        }

        return null;
    }

    private void SpawnLand(int x, int y)
    {
        float islandScale = islandToSpawnPrefab.transform.localScale.x;

        LandManager spawnedLand = Instantiate(islandToSpawnPrefab, new Vector3(islandScale * x, -5f, islandScale * y) , Quaternion.identity, transform);
        spawnedLand.Init(x, y);

        SpawnedLands.Add(spawnedLand);
    }

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

    public void TryEmpowerLandAtGhost()
    {
        if (EmpowerTokens <= 0)
        {
            Debug.Log("No more empower tokens");
            return;
        }

        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);

        LandManager hoveredLand = GetLand(spawnPosition.x, spawnPosition.y);

        if (hoveredLand == null)
        {
            Debug.Log("Can't empower at this ghost position");
            return;
        }

        hoveredLand.AddLevel(1);
        EmpowerTokens--;
    }

    public void TryWeakenLandAtGhost()
    {
        if (WeakenTokens <= 0)
        {
            Debug.Log("No more weaken tokens");
            return;
        }

        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);

        LandManager hoveredLand = GetLand(spawnPosition.x, spawnPosition.y);

        if (hoveredLand == null)
        {
            Debug.Log("Can't weaken at this ghost position");
            return;
        }

        hoveredLand.AddLevel(-1);
        WeakenTokens--;
    }

    public bool CanProceedFromEmpowerment()
    {
        return EmpowerTokens + WeakenTokens <= 0;
    }

    public void ContinueToEventSelection()
    {
        gameManager.ChangeState(GameState.EVENT_SELECTION);
    }

    public void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    public void AddBorder(IslandBorder border)
    {
        bordersList.Add(border);
    }

    public void RemoveConnectedBorders()
    {
        foreach (LandManager island in new List<LandManager>(SpawnedLands))
        {
            foreach (IslandBorder border in new List<IslandBorder>(bordersList))
            {
                if (island.GridPosition == border.WorldBorderPosition)
                {
                    Destroy(border.gameObject);
                    bordersList.Remove(border);
                }
            }
        }
    }

    public void PrepareForNextWave()
    {
        activeLandCount = SpawnedLands.Count;
        foreach (LandManager island in SpawnedLands)
        {
            island.EnemySpawner.WaveReset();
        }
    }

    public void DecrementActiveLandCount()
    {
        activeLandCount--;
        if (activeLandCount == 0)
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
        }
    }

    public void AssignBiomeToSpawnNext(Biome biome)
    {
        currentlySelectedBiome = biome;

        gameManager.ChangeState(GameState.LAND_PLACEMENT);
    }

    public void AssignNextEvent(WorldEvent worldEvent)
    {
        PrepareForNextWave();

        gameManager.ChangeState(GameState.PLAYING);
    }

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

        ghostLandTransform.position = GetLandPosition(gridPosition, ghostLandTransform.position.y);

        if(gameManager.CurrentState == GameState.LAND_PLACEMENT)
        {
            ghostLandTransform.GetComponent<MeshRenderer>().material = CanNewLandSpawnAt(gridPosition) ? greenTransparentMaterial : redTransparentMaterial;
        }

        if(gameManager.CurrentState == GameState.LAND_EMPOWERMENT)
        {
            ghostLandTransform.GetComponent<MeshRenderer>().material = GetLand(gridPosition.x, gridPosition.y) != null ? greenTransparentMaterial : redTransparentMaterial;
        }
    }
    #endregion

    #region Land Level Texts
    public void EnableLandLevelTexts()
    {
        foreach(LandManager land in SpawnedLands)
        {
            land.EnableLevelText();
        }
    }

    public void DisableLandLevelTexts()
    {
        foreach (LandManager land in SpawnedLands)
        {
            land.DisableLevelText();
        }
    }
    #endregion

    #region Progression Functions
    private void RestockTokens(int landCount)
    {
        EmpowerTokens = Mathf.CeilToInt(landCount / 2f);
        WeakenTokens = Mathf.FloorToInt(landCount / 2f);
    }
    #endregion
}

public enum Biome
{
    DREAM,
    FIRE,
    FOOD
}

public enum WorldEvent
{
    TEST
}
