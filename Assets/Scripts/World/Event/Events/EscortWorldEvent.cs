using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An NPC will run around the map for X minutes.
// Only land the NPC stands on spawn enemies,if they survive trigger EOW
public class EscortWorldEvent : BaseState
{
    private EventManager eventManager;
    private WorldManager worldManager;
    private EventsConfigSO eventsConfigSO;

    public EscortWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
        this.eventsConfigSO = eventsConfigSO;
    }

    public override void OnEnter()
    {
        Debug.Log($"Entered {GetType().Name} Event");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}
