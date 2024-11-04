using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileManager : MonoBehaviour
{
    private MasterLevelManager masterLevelManager;
    [Header("Linked Scripts")]
    [SerializeField] public GameObject placeObjectOnGrid;
    [SerializeField] public GameObject objFollowMouse;
    [SerializeField] public GameObject player;
    [Header("Misc Controls")]
    [SerializeField] public bool isInSkyView;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject tileCamera;
    public bool IsSelecting = false;

    private void Awake()
    {
        IsSelecting = false;
        masterLevelManager = FindAnyObjectByType<MasterLevelManager>();
        placeObjectOnGrid.SetActive(false);
        objFollowMouse.SetActive(false);
        playerCamera.SetActive(true);
        tileCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (AreAllWavesFinished() && IsSelecting == false)
        {
            IsSelecting = true;
            StartPlacing();
        }
    }
    
    public void StartPlacing()
    {
        player.SetActive(false);
        placeObjectOnGrid.SetActive(true);
        objFollowMouse.SetActive(true);
        playerCamera.SetActive(false);
        tileCamera.SetActive(true);
    }

    public void StopPlacing()
    {
        placeObjectOnGrid.SetActive(false);
        objFollowMouse.SetActive(false);
        playerCamera.SetActive(true);
        tileCamera.SetActive(false);
        player.SetActive(true);
    }

    private bool AreAllWavesFinished()
    {
        bool finished = true;

        foreach(IslandManager island in masterLevelManager.SpawnedIslands)
        {
            EnemySpawner enemyManager = island.EnemySpawner;

            if (!enemyManager.IsWaveFinished) finished = false;
        }

        return finished;
    }
}
