using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All lands spawn enemies for a certain amount of time, if the player survives that amount of time trigger EOW
public class SurvivalWorldEvent : BaseState
{
    private EventManager eventManager;
    private WorldManager worldManager;

    public SurvivalWorldEvent(EventManager eventManager, WorldManager worldManager)
    { 
        this.eventManager = eventManager;
        this.worldManager = worldManager;
    }

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        
    }
}