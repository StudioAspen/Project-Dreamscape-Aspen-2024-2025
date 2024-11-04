using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileManager : MonoBehaviour
{
    private MasterLevelManager masterLevelManager;
    [Header("Linked Scripts")]
    [SerializeField] public PlaceObjectOnGrid placeObjectOnGrid;
    [SerializeField] public ObjFollowMouse objFollowMouse;
    [Header("Misc Controls")]
    [SerializeField] public bool isInSkyView;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject tileCamera;
    public bool IsSelecting = false;

    private void Awake()
    {
        IsSelecting = false;
        masterLevelManager = FindAnyObjectByType<MasterLevelManager>();
        placeObjectOnGrid.enabled = false;
        objFollowMouse.enabled = false;
        playerCamera.SetActive(true);
        tileCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (AreAllWavesFinished() && IsSelecting == false)
        {
            IsSelecting = true;
        }
        StartPlacing();
    }
    
    public void StartPlacing()
    {
        placeObjectOnGrid.enabled = true;
        objFollowMouse.enabled = true;
        //playerCamera.SetActive(false);
        //tileCamera.SetActive(true);
    }

    public void StopPlacing()
    {
        placeObjectOnGrid.enabled = false;
        objFollowMouse.enabled = false;
        //playerCamera.SetActive(true);
        //tileCamera.SetActive(false);
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
