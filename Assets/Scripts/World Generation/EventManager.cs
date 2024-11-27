using System.Collections.Generic;
using TMPro;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using Unity.VisualScripting;

public enum WorldEvent
{
    START,
    SURVIVAL, // All lands spawn enemies for a certain amount of time, if the player survives that amount of time trigger EOW
    ZONES, // A 3x3 of lands are highlighted on the map. Enemies will only spawn from those lands, once they are all defeated trigger EOW
    PRIORITIES, // 3 Lands of the highest Level are selected. All lands will spawn enemies, once the enemies spawned from the specific lands chosen are defeated trigger EOW
    ESCORT, // An NPC will run around the map for 1 minute. Only land the NPC stands on spawn enemies,if they survive trigger EOW
    VISIT_ALL, // All land will light up. When the player steps on a land it will go way all lands will spawn enemies. Once all the lands have been touched by the player, trigger EOW
    DEFEND, // A stationary object will placed at the center of the land for 1 minute, Every 30 seconds it will go to a neighboring land. All lands will spawn enemies, if the object survives trigger EOW
}

public class EventManager : MonoBehaviour
{
    [SerializeField, Scene] public WorldManager worldManager;
    [SerializeField, Scene] public GameManager gameManager;

    [Header("Current Event")]
    [SerializeField] public WorldEvent CurrentWaveType = WorldEvent.START;

    [Header("Event Complete Bool")]
    [SerializeField] private bool EventClearStatus = false;

    private int activeLandCount;
    private int activePrioritiesCount;

    [SerializeField] private GameObject escortNPC;
    private float escortWaveTimer;
    private float escortWaveTimerLength = 180f; //temp value
    private bool startEscortWaveTimer = false;
    private EscortNpc npcScriptScriptReference;

    [SerializeField] private GameObject defendObject;
    private float defendWaveTimer;
    private float defendWaveTimerLength = 15f; //temp value
    private bool startDefendWaveTimer = false;
    private DefendObject defendObjectScriptScriptReference;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    void Start()
    {
        CurrentWaveType = WorldEvent.START;
        PrepareForNextWave();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && CurrentWaveType == WorldEvent.ESCORT) FinishEscortWave();
        if (Input.GetKeyDown(KeyCode.K) && CurrentWaveType == WorldEvent.DEFEND) FinishDefendWave();

        switch (CurrentWaveType)
        {
            case WorldEvent.START:
                // TODO: Check if all enemies have been killed
                if (activeLandCount == 0 && gameManager.CurrentState == GameState.PLAYING)
                {
                    WaveCompletion();
                }
                break;
            case WorldEvent.SURVIVAL:
                break;
            case WorldEvent.ZONES:
                // TODO: Check if all enemies in the 3 highest level islands have been killed
                if (EventClearStatus && gameManager.CurrentState == GameState.PLAYING)
                {
                    WaveCompletion();
                }
                break;
            case WorldEvent.PRIORITIES:
                break;
            case WorldEvent.ESCORT:
                if(startEscortWaveTimer)
                {
                    escortWaveTimer -= Time.deltaTime;
                    if(escortWaveTimer <= 0)
                    {
                        startEscortWaveTimer = false;
                        FinishEscortWave();
                    }
                }
                break;
            case WorldEvent.DEFEND:
                if (startDefendWaveTimer)
                {
                    defendWaveTimer -= Time.deltaTime;
                    if (defendWaveTimer <= 0)
                    {
                        startDefendWaveTimer = false;
                        FinishDefendWave();
                    }
                }
                break;
            case WorldEvent.VISIT_ALL:
                // TODO: Check if all lands have been visited
                if (worldManager.AreAllLandsVisited() && gameManager.CurrentState == GameState.PLAYING)
                {
                    WaveCompletion();
                }
                break;
            default:
                Debug.LogError("ERROR: No event is active");
                break;
        }
    }

    private void WaveCompletion()
    {
        gameManager.ChangeState(GameState.BIOME_SELECTION);
    }

    // Assigns the next world event and prepares for the next wave of enemies, then changes the game state to PLAYING.
    public void AssignNextEvent(WorldEvent NewWaveType)
    {
        CurrentWaveType = NewWaveType;
        PrepareForNextWave();
        gameManager.ChangeState(GameState.PLAYING);
    }

    #region Event Clear Status
    public void setEventClearStatus(bool status)
    {
        EventClearStatus = status;
    }
    #endregion

    #region Preperation Functions
    public void PrepareForNextWave() 
    {

        if(CurrentWaveType == WorldEvent.START) //START does not get a preperation function because it is the ugly duckling, we dont f with ugly ducklings
        {
            activeLandCount = 1;
        }

        if (CurrentWaveType == WorldEvent.SURVIVAL) //this will need to be changed
        {
            PrepareForSurvivalWave();
        }

        if (CurrentWaveType == WorldEvent.PRIORITIES) //this probably still works
        {
            PrepareForPrioritiesWave();
        }

        if (CurrentWaveType == WorldEvent.ESCORT)
        {
            PrepareForEscortWave();
        }

        if (CurrentWaveType == WorldEvent.DEFEND)
        {
            PrepareForDefendWave();
        }

        if (CurrentWaveType == WorldEvent.ZONES)
        {
            PrepareForZonesWave();
        }

        if (CurrentWaveType == WorldEvent.VISIT_ALL)
        {
            PrepareForVisitAllWave();
        }

 
    }

    private void PrepareForSurvivalWave()
    {
        activeLandCount = worldManager.SpawnedLands.Count;
        ResetSpawners();
    }

    private void PrepareForPrioritiesWave()
    {
        if (worldManager.SpawnedLands.Count <= 3)
        {
            foreach (LandManager land in worldManager.SpawnedLands) //WAIT ACTUALLY, IF THERE ARE LESS THAN 4 LANDS, THEN THE WAVE FUNCTIONS THE EXACT SAME AS THE SURVIVAL WAVE, SO THIS CAN ALL BE SCRAPPED
            {
                activePrioritiesCount += 1;
                land.EnemySpawner.IsPriority = true;
                land.EnemySpawner.WaveReset();
            }
            Debug.Log(activePrioritiesCount);
        }
        else //There are at least 4 lands present
        {
            activePrioritiesCount = 3;

            LandManager highestLevelLand = null;
            LandManager secondHighestLevelLand = null;
            LandManager thirdHighestLevelLand = null;

            foreach (LandManager land in worldManager.SpawnedLands)
            {
                if (highestLevelLand == null || land.Level > highestLevelLand.Level)
                {
                    thirdHighestLevelLand = secondHighestLevelLand;
                    secondHighestLevelLand = highestLevelLand;
                    highestLevelLand = land;
                }
                else if (secondHighestLevelLand == null || land.Level > highestLevelLand.Level)
                {
                    thirdHighestLevelLand = secondHighestLevelLand;
                    secondHighestLevelLand = land;
                }
                else if (thirdHighestLevelLand == null || land.Level > thirdHighestLevelLand.Level)
                {
                    thirdHighestLevelLand = land;
                }
            }

            highestLevelLand.EnemySpawner.IsPriority = true;
            secondHighestLevelLand.EnemySpawner.IsPriority = true;
            thirdHighestLevelLand.EnemySpawner.IsPriority = true;

            ResetSpawners();

        }
    }

    private void PrepareForEscortWave()
    {
        Player[] players = FindObjectsOfType<Player>(); //NEED TO ALTER EOW CONDITIONS SO THAT THEY CHECK IF PLAYERS ARE ALIVE

        if (players.Length == 0) //No players found
        {
            return;
        }
        else
        {
            ResetSpawners(); 

            int randomPlayerIndex;
            randomPlayerIndex = UnityEngine.Random.Range(0, players.Length);
            LandManager npcStartingLand = worldManager.GetLandByWorldPosition(players[randomPlayerIndex].transform.position);
            GameObject npcObject = Instantiate(escortNPC, worldManager.GetLandPositionByGridPosition(npcStartingLand.GridPosition, 1f), Quaternion.identity);

            npcScriptScriptReference = npcObject.GetComponent<EscortNpc>();

            escortWaveTimer = escortWaveTimerLength;
            startEscortWaveTimer = true;
        }
    }

    private void PrepareForVisitAllWave()
    {
        activeLandCount = worldManager.SpawnedLands.Count;
        ResetSpawners();
    }
    private void PrepareForZonesWave()
    {
        activeLandCount = worldManager.SpawnedLands.Count;
        ResetSpawners();
    }
    private void PrepareForDefendWave()
    {
        Player[] players = FindObjectsOfType<Player>(); //NEED TO ALTER EOW CONDITIONS SO THAT THEY CHECK IF PLAYERS ARE ALIVE

        if (players.Length == 0) //No players found
        {
            return;
        }
        else
        {
            ResetSpawners();

            int randomPlayerIndex;
            randomPlayerIndex = UnityEngine.Random.Range(0, players.Length);
            LandManager defendObjectStartingLand = worldManager.GetLandByWorldPosition(players[randomPlayerIndex].transform.position);
            GameObject worldDefendObject = Instantiate(defendObject, worldManager.GetLandPositionByGridPosition(defendObjectStartingLand.GridPosition, 1f), Quaternion.identity);

            defendObjectScriptScriptReference = worldDefendObject.GetComponent<DefendObject>();

            defendWaveTimer = defendWaveTimerLength;
            startDefendWaveTimer = true;

        }
    }
        #endregion

        #region Supporting FUN-ctions!!!!!!!

        public void ResetSpawners()
    {
        foreach (LandManager land in worldManager.SpawnedLands)
        {
            land.EnemySpawner.WaveReset();
        }
    }

    // Decrements the active land count and transitions the game to biome selection if all lands are processed.
    public void DecrementActiveLandCount()
    {
        activeLandCount--;
        if (activeLandCount == 0)
        {
            WaveCompletion();
        }
    }

    // Decrements the active priorities count and despawns all non-priority enemies if there are no active priorities left, 
    // then transitions the game to biome selection.
    public void DecrementActivePrioritiesCount()
    {
        activePrioritiesCount--;
        if (activePrioritiesCount == 0)
        {
            foreach (LandManager land in worldManager.SpawnedLands)
            {
                if (!land.EnemySpawner.IsPriority)
                {
                    land.EnemySpawner.DespawnAllEnemies();
                }
                else
                {
                    land.EnemySpawner.IsPriority = false;
                }
            }
            WaveCompletion();
        }
    }

    public void FinishEscortWave()
    {
        foreach(LandManager land in worldManager.SpawnedLands)
        {
            land.EnemySpawner.NpcPresent = false;
            land.EnemySpawner.CurrencyTimerActive = false;
            land.EnemySpawner.DespawnAllEnemies();
        }

        npcScriptScriptReference.Destroy();


        WaveCompletion();
    }


    public void FinishDefendWave()
    {
        foreach (LandManager land in worldManager.SpawnedLands)
        {
            land.EnemySpawner.DespawnAllEnemies();
        }
        defendObjectScriptScriptReference.Destroy();
        WaveCompletion();
    }


    #endregion


}