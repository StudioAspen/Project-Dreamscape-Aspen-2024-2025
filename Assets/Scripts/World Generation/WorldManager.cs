using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using KBCore.Refs;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private MasterLevelManager masterLevelManager;

    [Header("Settings")]
    public bool IsSelecting;
    private int activeLandCount;

    [HideInInspector] public UnityEvent OnWaveFinished = new UnityEvent();

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Start()
    {
        activeLandCount = 1;
    }

    public void PrepareForNextWave() 
    {
        activeLandCount = masterLevelManager.SpawnedIslands.Count;
        foreach (IslandManager island in masterLevelManager.SpawnedIslands) 
        {
            island.EnemySpawner.WaveReset();
        }
    }

    public void DecrementActiveLandCount()
    {
        activeLandCount--;
        if (activeLandCount == 0)
        {
            IsSelecting = true;

            OnWaveFinished?.Invoke();
        }
    }

}
