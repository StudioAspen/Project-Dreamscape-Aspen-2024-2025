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

    [Header("References")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private List<LandBorder> borders;
    [SerializeField] private Transform bodyContentTransform;
    [field: SerializeField] public Material SkyBoxMaterial { get; private set; }

    [field: Header("Settings")]
    [field: SerializeField] public Vector2Int GridPosition { get; private set; }
    [field: SerializeField] public string WwiseStateName { get; private set; }
    public Biome Biome { get; private set; }

    [field: Header("Progression Tracking")]
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int LevelDifference { get; private set; } = 0;
    [SerializeField] private int minLevel = -5;
    [SerializeField] private int maxLevel = 10;
    
    /// <summary>
    /// Used for random selections.
    /// </summary>
    public float Weight => 1f/(1f + maxLevel - Level);

    /// <summary>
    /// Initializes the LandManager with the given grid position.
    /// </summary>
    /// <param name="gridPosition">The grid position to initialize with.</param>
    public void Init(Vector2Int gridPosition, Biome biomeType)
    {
        GridPosition = gridPosition;

        Biome = biomeType;
    }

    private void Awake()
    {
        EnemySpawner = GetComponent<EnemySpawner>();
    }

    private void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();

        // Give the body a random 90 degree rotation
        bodyContentTransform.transform.localRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);

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
    /// <returns>Whether the land's level was sucessfully changed</returns>
    public bool TryAddLevel(int amount)
    {
        // If at minimum
        if(Level + amount < minLevel)
        {
            return false;
        }

        // If at maximum
        if(Level + amount > maxLevel)
        {
            return false;
        }

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

        return true;
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
        TryAddLevel(-LevelDifference);
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

    public void UpdateWwiseState()
    {
        Debug.Log($"Setting Wwise Biome state to {WwiseStateName}");
        AkSoundEngine.SetState("Biome", WwiseStateName);
    }
}
    