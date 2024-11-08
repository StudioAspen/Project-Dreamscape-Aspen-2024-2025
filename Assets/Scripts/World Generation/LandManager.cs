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
    [field: SerializeField] public Biome Biome { get; private set; }

    [field: Header("Progression Tracking")]
    [field: SerializeField] public int LevelDifference { get; private set; } = 0;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    public void Init(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }

    void Start()
    {
        Level = 1;

        levelText.text = $"{Level}";

        InitializeBorders();

        //transform.DOMoveY(-5, 0.5f).SetEase(Ease.InBounce).OnComplete(()=>StartCoroutine(OnCompleteSpawn()));
        StartCoroutine(OnCompleteSpawn());
    }

    private IEnumerator OnCompleteSpawn()
    {
        worldManager.RemoveConnectedBorders();

        yield return null;

        worldManager.BuildNavMesh();

        EnemySpawner.CanSpawn = true;
    }

    private void InitializeBorders()
    {
        foreach(LandBorder border in borders)
        {
            border.SetWorldBorderPosition(GridPosition);
            worldManager.AddBorder(border);
        }
    }

    public void AddLevel(int amount)
    {
        Level += amount;
        LevelDifference += amount;

        levelText.text = $"{Level}";
        
        if(Mathf.Abs(LevelDifference) > 0)
        {
            levelText.color = LevelDifference > 0 ? Color.green : Color.red;
        }
        else
        {
            levelText.color = Color.black;
        }
    }

    public void ResetLevelDifference()
    {
        LevelDifference = 0;
        levelText.color = Color.black;
    }

    public void UndoLevelChanges()
    {
        AddLevel(-LevelDifference);
        ResetLevelDifference();
    }

    public Transform GetRandomEnemySpawn()
    {
        int randomIndex = Random.Range(0, enemySpawnPoints.Count);
        return enemySpawnPoints[randomIndex];
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
    