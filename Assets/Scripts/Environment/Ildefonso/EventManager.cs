using System.Collections.Generic;
using TMPro;
using KBCore.Refs;
using UnityEngine;

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
    private bool eventActive = true;

    [Header("Event: Debug UI")]
    [SerializeField] private TMP_Text eventText;

    public WorldEvent CurrentEvent { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentEvent(WorldEvent.START);
    }

    // Update is called once per frame
    void Update()
    {
        //Event Debug UI
        eventText.text = $"Event: {CurrentEvent}";

        //idk yet lol, all code in WorldManager so far
        if(CurrentEvent == WorldEvent.START)
        {
            
        }
    }

    //set the current event ENUM
    public void SetCurrentEvent(WorldEvent newEvent)
    {
        CurrentEvent = newEvent;
    }
}

