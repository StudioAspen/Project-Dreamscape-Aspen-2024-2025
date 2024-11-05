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
    [SerializeField, Self] private ProgressionManager progressionManager; // nav mesh surface for pathfinding

    [field: Header("Pseudo Grid")]
    public List<LandManager> SpawnedLands { get; private set; } = new List<LandManager>(); // a list of all currently spawned lands
    private List<IslandBorder> bordersList = new List<IslandBorder>();
    private int activeLandCount;

    [Header("Land Position Selection")]
    [SerializeField] private LandManager islandToSpawnPrefab;
    [SerializeField] private ObjectPooler spherePooler;
    [SerializeField] private Transform ghostLandTransform;
    [SerializeField] private Material redTransparentMaterial;
    [SerializeField] private Material greenTransparentMaterial;
    public List<SelectionSphere> CurrentSelectionSpheres { get; private set; } = new List<SelectionSphere>(); // a list of all currently spawned spheres

    [field: Header("Biome Selection")]
    private Biome currentlySelectedBiome = Biome.DREAM;

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
        HandleMouseInput();
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

    private SelectionSphere GetMouseLookSelectionSphere()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        bool didHit = Physics.Raycast(mouseRay, out hit, Mathf.Infinity, LayerMask.GetMask("SelectionSphere"));

        if (!didHit) return null;

        return hit.transform.GetComponent<SelectionSphere>();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectionSphere selectionSphere = GetMouseLookSelectionSphere();

            if (selectionSphere == null) return;
            if (!selectionSphere.CanBeSelected) return;

            SpawnLand(selectionSphere.DesiredIslandSpawnPosition.x, selectionSphere.DesiredIslandSpawnPosition.y);
            DeleteAllSelectionSpheres();

            PrepareForNextWave();
        }
    }

    private void SpawnLand(int x, int y)
    {
        float islandScale = islandToSpawnPrefab.transform.localScale.x;

        LandManager spawnedLand = Instantiate(islandToSpawnPrefab, new Vector3(islandScale * x, -5f, islandScale * y) , Quaternion.identity, transform);
        spawnedLand.Init(x, y);

        SpawnedLands.Add(spawnedLand);

        PrepareForNextWave();
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

        gameManager.ChangeState(GameState.LAND_EMPOWERMENT);
    }

    public void TryEmpowerLandAtGhost()
    {
        Vector2Int spawnPosition = GetGridPosition(ghostLandTransform.position);
        if (GetLand(spawnPosition.x, spawnPosition.y) == null)
        {
            Debug.Log("Can't empower at this ghost position");
            return;
        }

        //gameManager.ChangeState(GameState.EVENT_SELECTION);
        gameManager.ChangeState(GameState.PLAYING);
    }

    public void SpawnSelectionSpheres() 
    {
        float islandScale = islandToSpawnPrefab.transform.localScale.x;

        DeleteAllSelectionSpheres(); //just in case spheres still exist

        for (int i = 0; i < bordersList.Count; i++)
        {
            Vector3 spherePos = islandScale * new Vector3(bordersList[i].WorldBorderPosition.x, 0f, bordersList[i].WorldBorderPosition.y);

            SelectionSphere newSphere = spherePooler.SpawnObject<SelectionSphere>(spherePos + 20f * Vector3.up);
            newSphere.SetDesiredIslandSpawn(bordersList[i].WorldBorderPosition);

            CurrentSelectionSpheres.Add(newSphere);
        }

    }

    private void DeleteAllSelectionSpheres()
    {
        foreach(SelectionSphere sphere in new List<SelectionSphere>(CurrentSelectionSpheres))
        {
            spherePooler.ReleaseObject(sphere.gameObject);
        }

        CurrentSelectionSpheres.Clear();
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
}

public enum Biome
{
    DREAM,
    FIRE,
    FOOD
}
