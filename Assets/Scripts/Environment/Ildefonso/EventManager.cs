
using System.Collections.Generic;
using TMPro;
using KBCore.Refs;
using UnityEngine;
using System.Runtime.CompilerServices;



public enum EventType
{
    START /*ONLY AT START OF GAME || All lands spawn enemies, if the player kills all trigger EOW*/,

    SURVIVAL /*All lands spawn enemies for a certain amount of time, if the player survives that amount of time trigger EOW*/,
    ZONES /*A 3x3 of lands are highlighted on the map. Enemies will only spawn from those lands, once they are all defeated trigger EOW*/,
    PRIORITIES /* 3 Lands of the highest Level are selected. All lands will spawn enemies, once the enemies spawned from the specific lands chosen are defeated trigger EOW*/,
    ESCORT /*An NPC will run around the map for 1 minute. Only land the NPC stands on spawn enemies,if they survive trigger EOW*/,
    VISIT_ALL /*All land will light up. When the player steps on a land it will go way all lands will spawn enemies. Once all the lands have been touched by the player, trigger EOW*/,
    DEFEND /*A stationary object will placed at the center of the land for 1 minute, Every 30 seconds it will go to a neighboring land. All lands will spawn enemies, if the object survives trigger EOW*/,
}

public class EventManager : MonoBehaviour
{
    private WorldManager worldManager;
    private bool eventActive = true;

    [Header("Event: Debug UI")]
    [SerializeField] private TMP_Text eventText;

    public EventType CURRENT_EVENT;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        worldManager = FindObjectOfType<WorldManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CURRENT_EVENT = EventType.START;
    }

    // Update is called once per frame
    void Update()
    {
        //Event Debug UI
        eventText.text = "Event: " + CURRENT_EVENT.ToString();

        //idk yet lol, all code in WorldManager so far
        if(CURRENT_EVENT == EventType.START)
        {
            
        }
    }



    //return the current event ENUM
    public EventType GetCurrentEvent()
    {
        return CURRENT_EVENT;
    }

    //return the current event ENUM as string
    public string GetCurrentEventString()
    {
        return CURRENT_EVENT.ToString();
    }

    //set the current event ENUM
    public void SetCurrentEvent(EventType newEvent)
    {
        CURRENT_EVENT = newEvent;
    }
}

