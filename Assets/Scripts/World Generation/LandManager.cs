using DG.Tweening;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class LandManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [field: SerializeField, Self] public EnemySpawner EnemySpawner { get; private set; }
    [SerializeField, Self] private NavMeshSurface navMeshSurface;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private LandBorder[] borders;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [field: Header("Settings")]
    [field: SerializeField] public Vector2Int GridPosition { get; private set; }
    [field: SerializeField] public int Level { get; private set; }

    [field: Header("Progression Tracking")]
    [field: SerializeField] public int LevelDifference { get; private set; } = 0;

    // This function is called when the script is loaded or when values in the Inspector are changed.
    // It validates the references assigned to the script to ensure they are correctly set.
    private void OnValidate()
    {
        this.ValidateRefs();
    }

    // This function initializes the grid position of the land.
    // It sets the GridPosition property using the provided x and y coordinates.
    // It is called to set the initial grid position, especially useful for the first land to spawn.
    public void Init(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }

    // This function is called when the script starts.
    // It sets the initial level of the land to 1 and updates the levelText UI element to display the level.
    // It also initializes the borders for the land and begins the spawn process by calling OnCompleteSpawn.
    void Start()
    {
        Level = 1;
        levelText.text = $"{Level}";

        InitializeBorders();

        StartCoroutine(OnCompleteSpawn());
    }

    // This is a coroutine function that completes the spawn process of the land.
    // It removes any connected borders between lands, waits for the next frame,
    // rebuilds the navigation mesh, and enables enemy spawning by setting the CanSpawn flag to true.
    private IEnumerator OnCompleteSpawn()
    {
        worldManager.RemoveConnectedBorders();

        yield return null;

        worldManager.BuildNavMesh();

        EnemySpawner.CanSpawn = true;
    }

    // This function initializes the borders for the land.
    // It loops through each LandBorder in the 'borders' array (initialized in the editor),
    // sets their world position based on the current land's GridPosition, 
    // and registers them with the WorldManager to track them as part of the world layout.
    private void InitializeBorders()
    {
        foreach (LandBorder border in borders)
        {
            border.SetWorldBorderPosition(GridPosition);
            worldManager.AddBorder(border);
        }
    }

    // This function is used to increase or decrease the land's level by the given amount.
    // It also tracks the level difference for progression purposes and updates the levelText UI element.
    // The levelText color is updated based on whether the level has increased (green) or decreased (red).
    public void AddLevel(int amount)
    {
        Level += amount;
        LevelDifference += amount;

        levelText.text = $"{Level}";

        if (Mathf.Abs(LevelDifference) > 0)
        {
            levelText.color = LevelDifference > 0 ? Color.green : Color.red;
        }
        else
        {
            levelText.color = Color.black;
        }
    }

    // This function resets the LevelDifference property to 0.
    // It also resets the levelText color to black, indicating no change in the land's level.
    public void ResetLevelDifference()
    {
        LevelDifference = 0;
        levelText.color = Color.black;
    }

    // This function undoes the changes made to the land's level by reversing the LevelDifference.
    // It calls AddLevel to apply the reverse of the level change and then resets the LevelDifference.
    public void UndoLevelChanges()
    {
        AddLevel(-LevelDifference);
        ResetLevelDifference();
    }

    // This function returns a random spawn point for enemies.
    // It selects a random index from the 'enemySpawnPoints' list and returns the corresponding Transform,
    // which can be used to spawn enemies at that location.
    public Transform GetRandomEnemySpawn()
    {
        int randomIndex = Random.Range(0, enemySpawnPoints.Count);
        return enemySpawnPoints[randomIndex];
    }

    // This function enables the levelText UI element, making it visible on the screen.
    public void EnableLevelText()
    {
        levelText.gameObject.SetActive(true);
    }

    // This function disables the levelText UI element, hiding it from the screen.
    public void DisableLevelText()
    {
        levelText.gameObject.SetActive(false);
    }
}
