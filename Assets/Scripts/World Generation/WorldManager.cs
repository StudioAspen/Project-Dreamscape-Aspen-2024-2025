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
    public List<IslandManager> SpawnedLands { get; private set; } = new List<IslandManager>(); // a list of all currently spawned lands
    private List<IslandBorder> bordersList = new List<IslandBorder>();
    private int activeLandCount;

    [Header("Land Position Selection")]
    [SerializeField] private IslandManager islandToSpawnPrefab;
    [SerializeField] private ObjectPooler spherePooler;
    public List<SelectionSphere> CurrentSelectionSpheres { get; private set; } = new List<SelectionSphere>(); // a list of all currently spawned spheres
    private bool isSelecting;

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
        SpawnedLands.Add(GetComponentInChildren<IslandManager>());

        BuildNavMesh();

        activeLandCount = 1;
    }

    void Update()
    {
        HandleMouseInput();
    }

    public IslandManager GetIsland(int x, int y)
    {
        foreach(IslandManager island in new List<IslandManager>(SpawnedLands))
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

            SpawnIsland(selectionSphere.DesiredIslandSpawnPosition.x, selectionSphere.DesiredIslandSpawnPosition.y);
            DeleteAllSelectionSpheres();

            isSelecting = false;
            PrepareForNextWave();
        }
    }

    private void SpawnIsland(int x, int y)
    {
        float islandScale = islandToSpawnPrefab.transform.localScale.x;

        IslandManager spawnedIsland = Instantiate(islandToSpawnPrefab, new Vector3(islandScale * x, -15f, islandScale * y) , Quaternion.identity, transform);
        spawnedIsland.Init(x, y);

        SpawnedLands.Add(spawnedIsland);
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
        foreach (IslandManager island in new List<IslandManager>(SpawnedLands))
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
        foreach (IslandManager island in SpawnedLands)
        {
            island.EnemySpawner.WaveReset();
        }
    }

    public void DecrementActiveLandCount()
    {
        activeLandCount--;
        if (activeLandCount == 0)
        {
            isSelecting = true;

            gameManager.ChangeState(GameState.BIOME_SELECTION);
        }
    }

    public void AssignBiomeToSpawnNext(Biome biome)
    {
        currentlySelectedBiome = biome;

        gameManager.ChangeState(GameState.LAND_PLACEMENT);
    }
}

public enum Biome
{
    DREAM,
    FIRE,
    FOOD
}
