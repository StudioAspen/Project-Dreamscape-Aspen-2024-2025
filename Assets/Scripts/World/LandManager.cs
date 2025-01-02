using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class LandManager : MonoBehaviour
{
    private WorldManager worldManager;
    public EnemySpawner EnemySpawner { get; private set; }
    private NavMeshSurface navMeshSurface;

    [Header("References")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private List<LandBorder> borders;

    [field: Header("Settings")]
    [field: SerializeField] public Vector2Int GridPosition { get; private set; }
    [field: SerializeField] public int Level { get; private set; }

    [field: Header("Progression Tracking")]
    [field: SerializeField] public int LevelDifference { get; private set; } = 0;

    /// <summary>
    /// Initializes the LandManager with the given grid position.
    /// </summary>
    /// <param name="gridPosition">The grid position to initialize with.</param>
    public void Init(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
    }

    private void Awake()
    {
        worldManager = FindObjectOfType<WorldManager>();
        EnemySpawner = GetComponent<EnemySpawner>();
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        Level = 1;

        levelText.text = $"{Level}";

        InitializeBorders();

        StartCoroutine(OnCompleteSpawn());
    }

    /// <summary>
    /// Coroutine that is called when the spawn is complete.
    /// Removes connected borders, waits for one frame, and then builds the NavMesh.
    /// </summary>
    private IEnumerator OnCompleteSpawn()
    {
        worldManager.RemoveConnectedBorders();

        yield return null;

        worldManager.BuildNavMesh();
    }

    /// <summary>
    /// Initializes the borders of the land with the current grid position.
    /// Adds the borders to the world manager.
    /// </summary>
    private void InitializeBorders()
    {
        foreach (LandBorder border in borders)
        {
            border.SetWorldBorderPosition(GridPosition);
            worldManager.AddBorder(border);
        }
    }

    /// <summary>
    /// Adds the specified amount to the current level and updates the level text and color accordingly.
    /// Also updates the LevelDifference which is used to track the changes made to the current level.
    /// </summary>
    /// <param name="amount">The amount to add to the level.</param>
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

    /// <summary>
    /// Resets the level difference to zero and sets the level text color to black.
    /// </summary>
    public void ResetLevelDifference()
    {
        LevelDifference = 0;
        levelText.color = Color.black;
    }

    /// <summary>
    /// Undoes the changes made to the level by subtracting the LevelDifference and resetting it to zero.
    /// </summary>
    public void UndoLevelChanges()
    {
        AddLevel(-LevelDifference);
        ResetLevelDifference();
    }

    public void EnableLevelText()
    {
        levelText.gameObject.SetActive(true);
    }

    public void DisableLevelText()
    {
        levelText.gameObject.SetActive(false);
    }
}
    