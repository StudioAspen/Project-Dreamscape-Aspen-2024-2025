using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public enum Biome
{
    DREAM,
    FIRE,
    FOOD,
    BIOME4
}

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private NavMeshSurface navMeshSurface; // nav mesh surface for pathfinding

    [field: Header("Pseudo Grid")]
    public List<LandManager> SpawnedLands { get; private set; } = new List<LandManager>(); // a list of all currently spawned lands
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
    private WorldEvent currentEventSelection;

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

    #region Grid Functions
    public LandManager GetLandByGridPosition(int x, int y)
    {
        foreach (LandManager land in new List<LandManager>(SpawnedLands))
        {
            if (land.GridPosition.x == x && land.GridPosition.y == y) return land;
        }

        return null;
    }

    public LandManager GetLandByGridPosition(Vector2Int gridPosition)
    {
        return GetLandByGridPosition(gridPosition.x, gridPosition.y);
    }

    public Vector2Int GetGridPosition(Vector3 worldPos)
    {
        float landScale = landToSpawnPrefab.transform.localScale.x;

        Vector3 floatGridPosition = worldPos / landScale;

        return new Vector2Int(Mathf.RoundToInt(floatGridPosition.x), Mathf.RoundToInt(floatGridPosition.z));
    }

    public LandManager GetLandByWorldPosition(Vector3 worldPos)
    {
        Vector2Int gridPosition = GetGridPosition(worldPos);

        return GetLandByGridPosition(gridPosition.x, gridPosition.y);
    }

    public Vector3 GetLandPosition(Vector2Int gridPosition, float height)
    {
        float landScale = landToSpawnPrefab.transform.localScale.x;

        return new Vector3(landScale * gridPosition.x, height, landScale * gridPosition.y);
    }
    #endregion

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

    private void SpawnLand(int x, int y)
    {
        float landScale = landToSpawnPrefab.transform.localScale.x;

        LandManager spawnedLand = Instantiate(landToSpawnPrefab, new Vector3(landScale * x, -5f, landScale * y) , Quaternion.identity, transform);
        spawnedLand.Init(x, y);

        SpawnedLands.Add(spawnedLand);
    }

    public void TrySpawnLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);
        if (!CanNewLandSpawnAt(spawnPosition))
        {
            Debug.LogError("Can't spawn new land at this ghost position");
            return;
        }

        DisableGhostLand();
        SpawnLand(spawnPosition.x, spawnPosition.y);

        RestockTokens(SpawnedLands.Count);
        gameManager.ChangeState(GameState.LAND_EMPOWERMENT);
    }

    public void ContinueToEventSelection()
    {
        ResetLandLevelDifferences();

        gameManager.ChangeState(GameState.EVENT_SELECTION);
    }

    public void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    public void AddBorder(LandBorder border)
    {
        bordersList.Add(border);
    }

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

    public void PrepareForNextWave()
    {
        activeLandCount = SpawnedLands.Count;
        foreach (LandManager land in SpawnedLands)
        {
            land.EnemySpawner.WaveReset();
        }
    }

    public void DecrementActiveLandCount()
    {
        
        activeLandCount--;
        //OLD WAVE CLEAR SYSTEM (WHEN EVENTS ARENT IMPLEMENTED JUST CHECKS IF ALL ENEMIES ARE KILLED)
        /*if (activeLandCount == 0)
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
        }*/
    }

    //Needed for EventManager - Ildefonso Marrero (Environment Team)
    public int GetActiveLandCount()
    {
        return activeLandCount;
    }

    public void AssignBiomeToSpawnNext(Biome biome)
    {
        currentBiomeSelection = biome;

        gameManager.ChangeState(GameState.LAND_PLACEMENT);
    }

    public void AssignNextEvent(WorldEvent worldEvent)
    {
        // should move this to EventManager
        currentEventSelection = worldEvent;

        PrepareForNextWave(); // Replace this with start next event instead of PrepareForNextWave()

        gameManager.ChangeState(GameState.PLAYING);
    }

    public WorldEvent getCurrentEventSelection()
    {
        return currentEventSelection;
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
            ghostLandTransform.GetComponent<MeshRenderer>().material = GetLandByGridPosition(gridPosition.x, gridPosition.y) != null ? greenTransparentMaterial : redTransparentMaterial;
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

    public void TryEmpowerLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);

        LandManager hoveredLand = GetLandByGridPosition(spawnPosition.x, spawnPosition.y);

        if (hoveredLand == null)
        {
            Debug.LogError("Can't empower at this land");
            return;
        }

        if (hoveredLand.LevelDifference >= 0 && EmpowerTokens <= 0)
        {
            Debug.LogError("Not enough empower tokens");
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

    public void TryWeakenLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);

        LandManager hoveredLand = GetLandByGridPosition(spawnPosition.x, spawnPosition.y);

        if (hoveredLand == null)
        {
            Debug.LogError("Can't weaken at this land");
            return;
        }

        if (hoveredLand.LevelDifference <= 0 && WeakenTokens <= 0)
        {
            Debug.LogError("Not enough weaken tokens");
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

    public bool CanProceedFromEmpowerment()
    {
        return EmpowerTokens + WeakenTokens <= 0;
    }

    private void ResetLandLevelDifferences()
    {
        foreach(LandManager land in SpawnedLands)
        {
            land.ResetLevelDifference();
        }
    }

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
