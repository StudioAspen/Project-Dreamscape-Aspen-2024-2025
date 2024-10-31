using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    private EventManager eventManager;
    private MasterLevelManager masterLevelManager;

    [Header("Misc Controls")]
    [SerializeField] public bool isInSkyView;

    [Header("Island Selection")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject islandSelectCamera;
    public bool IsIslandSelecting;
    public bool IsEventSelecting;

    private void Awake()
    {
        masterLevelManager = FindAnyObjectByType<MasterLevelManager>();
        eventManager = FindAnyObjectByType<EventManager>();
    }

    void Update()
    {
        //EVENT SYSTEM - Ildefonso Marrero
        /*START COMPLETE*/
        if (eventManager.GetCurrentEvent() == EventType.START) //if the event is START, check for all enemies killed
        {
            //check if all enemies have been killed, if so, start the island selection
            if (AreAllWavesFinished() && !IsIslandSelecting && !IsEventSelecting)
            {
                IsIslandSelecting = true;
                FindObjectOfType<IslandSelectUI>().PrepareIslandSelection();
            }
        }
        else if (eventManager.GetCurrentEvent() == EventType.VISIT_ALL) //if the event is VISIT_ALL, check for all islands visited
        {
            //check if all islands have been visited
            bool VISITED_ALL = true;
            for (int i = 0; i < masterLevelManager.SpawnedIslands.Count; i++)
            {
                if (masterLevelManager.SpawnedIslands[i].IsVisited == false)
                {
                    VISITED_ALL = false;
                }
            }
            //if all islands have been visited, start the island selection
            if (VISITED_ALL && !IsIslandSelecting && !IsEventSelecting)
            {
                IsIslandSelecting = true;
                FindObjectOfType<IslandSelectUI>().PrepareIslandSelection();
            }
        }
        else if (eventManager.GetCurrentEvent() == EventType.ZONES) //if the event is ZONES, check for all enemies killed in the 3x3 grid
        {
            //TODO: Check if all enemies in the 3x3 grid have been killed
        }
        else if (eventManager.GetCurrentEvent() == EventType.SURVIVAL) //if the event is SURVIVAL, check for timer to end
        {
            //TODO: Check if the timer has ended
        }
        else if (eventManager.GetCurrentEvent() == EventType.PRIORITIES) //if the event is PRIORITIES, check for all enemies killed in the 3 highest level islands
        {
            //TODO: Check if all enemies in the 3 highest level islands have been killed
        }
        else if (eventManager.GetCurrentEvent() == EventType.ESCORT) //if the event is ESCORT, check for timer to end AND NPC survival
        {
            //TODO: Check if the timer has ended AND NPC survival
        }
        else if (eventManager.GetCurrentEvent() == EventType.DEFEND) //if the event is DEFEND, check for timer to end AND object survival
        {
            //TODO: Check if the timer has ended AND object survival
        }else
        {
            Debug.Log("ERROR: No event is active");
        }

        /*        if (Input.GetKeyDown(KeyCode.Q) && isInSkyView == false) 
                {
                    islandTimertext.SetActive(true);
                    islandSelectCamera.SetActive(true);
                    playerCamera.SetActive(false);
                    isInSkyView = true;
                }

                if (Input.GetKeyDown(KeyCode.E) && isInSkyView == true)
                {

                    islandSelectCamera.SetActive(false);
                    playerCamera.SetActive(true);
                    islandTimertext.SetActive(false);
                    isInSkyView = false;
                }*/
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

    public void PrepareForNextWave() 
    {
        foreach (IslandManager island in masterLevelManager.SpawnedIslands) 
        {
            island.LevelUp();
            island.EnemySpawner.WaveReset();
        }
    }

    public void IslandSelectComplete()
    {
        IsIslandSelecting = false;
        IsEventSelecting = true;
        FindObjectOfType<EventSelectUI>().PrepareEventSelection();
    }
}
