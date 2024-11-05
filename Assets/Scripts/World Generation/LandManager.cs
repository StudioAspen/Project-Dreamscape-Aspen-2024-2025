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
    [SerializeField] private IslandBorder[] borders;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [field: Header("Settings")]
    [field: SerializeField] public Vector2Int GridPosition { get; private set; }
    [field:SerializeField] public int Level { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
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
        foreach(IslandBorder border in borders)
        {
            border.SetWorldBorderPosition(GridPosition);
            worldManager.AddBorder(border);
        }
    }

    public void LevelUp()
    {
        Level += 1;

        levelText.text = $"{Level}";
    }

    public void Init(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
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
    