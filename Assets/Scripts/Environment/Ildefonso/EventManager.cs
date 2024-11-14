using System.Collections.Generic;
using TMPro;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public enum WorldEvent
{
    START, // ONLY AT START OF GAME || All lands spawn enemies, if the player kills all trigger EOW
    SURVIVAL, // All lands spawn enemies for a certain amount of time, if the player survives that amount of time trigger EOW
    ZONES, // A 3x3 of lands are highlighted on the map. Enemies will only spawn from those lands, once they are all defeated trigger EOW
    PRIORITIES, // 3 Lands of the highest Level are selected. All lands will spawn enemies, once the enemies spawned from the specific lands chosen are defeated trigger EOW
    ESCORT, // An NPC will run around the map for 1 minute. Only land the NPC stands on spawn enemies,if they survive trigger EOW
    VISIT_ALL, // All land will light up. When the player steps on a land it will go way all lands will spawn enemies. Once all the lands have been touched by the player, trigger EOW
    DEFEND, // A stationary object will placed at the center of the land for 1 minute, Every 30 seconds it will go to a neighboring land. All lands will spawn enemies, if the object survives trigger EOW
}

public class EventManager : MonoBehaviour
{
    [SerializeField, Scene] private WorldManager worldManager;
    [SerializeField, Scene] private GameManager gameManager;

    [Header("Debugging")]
    [SerializeField] private bool IsDebugging = true;

    [Header("Event: Debug UI")]
    [SerializeField] private TMP_Text eventText;

    [field: Header("Is Booleans")]
    [SerializeField] private bool IsSelecting { get; set; }
    [SerializeField] private bool IsIslandSelecting { get; set; }
    [SerializeField] private bool IsEventSelecting { get; set; }

    [Header("Event Complete Bool")]
    [SerializeField] private bool EventClearStatus = false;

    [Header("Current Event")]
    [SerializeField] private WorldEvent CurrEvent;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Event Debug UI
        eventText.text = $"Event: {worldManager.getCurrentEventSelection()}";

        CurrEvent = worldManager.getCurrentEventSelection();


        // Check for current event and handle accordingly
        switch (worldManager.getCurrentEventSelection())
        {
            case WorldEvent.START:
                // TODO: Check if all enemies have been killed
                if (worldManager.GetActiveLandCount() == 0 && gameManager.CurrentState == GameState.PLAYING)
                {
                    EventCompletion();
                }
                
                break;
            case WorldEvent.SURVIVAL:
                // TODO: Check if the timer has ended
                if (false)
                {
                    EventCompletion();
                }
                
                break;
            case WorldEvent.ZONES:
                // TODO: Check if all enemies in the 3x3 grid have been killed
                if (false)
                {
                    EventCompletion();
                }
                
                break;
            case WorldEvent.PRIORITIES:
                // TODO: Check if all enemies in the 3 highest level islands have been killed
                if (false)
                {
                    EventCompletion();
                }
                
                break;
            case WorldEvent.ESCORT:
                // TODO: Check if the timer has ended AND NPC survival
                if (false)
                {
                    EventCompletion();
                }
                
                break;
            case WorldEvent.DEFEND:
                // TODO: Check if the timer has ended AND object survival
                if (false)
                {
                    EventCompletion();
                }
                
                break;
            default:
                Debug.LogError("ERROR: No event is active");
                break;
        }

        
    }

    private bool AreAllWavesFinished()
    {
        // TODO: Implement the logic to check if all waves are finished
        return false;
    }

    private void EventCompletion()
    {
        // if Event is complete, change state to BIOME_SELECTION
        gameManager.ChangeState(GameState.BIOME_SELECTION);
    }

    public void setEventClearStatus(bool status)
    {
        if (IsDebugging)
        {
            Debug.Log($"Event Clear Status Set to: {status}");
            EventClearStatus = status;
        }
    }

}

