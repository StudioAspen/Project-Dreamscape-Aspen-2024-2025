using System.Collections.Generic;
using UnityEngine;

public class WorldEvent
{
    /// <summary>
    /// The event manager that manages the event.
    /// Change and clear events here.
    /// Add coroutines through this mono behaviour.
    /// </summary>
    private protected EventManager eventManager;
    /// <summary>
    /// The world manager that manages the lands and grid. Use this to access information about lands.
    /// </summary>
    private protected WorldManager worldManager;
    /// <summary>
    /// The events config scriptable object that contains the default configs for events.
    /// </summary>
    private protected EventsConfigSO eventsConfigSO;

    /// <summary>
    /// The list of enemy spawning coroutines that are currently running.
    /// Add coroutines that are started through the eventManager mono behaviour here.
    /// </summary>
    private protected List<Coroutine> enemySpawningCoroutines = new List<Coroutine>();

    public WorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
        this.eventsConfigSO = eventsConfigSO;
    }

    /// <summary>
    /// Called once when starting the event.
    /// </summary>
    public virtual void OnStarted() { }

    /// <summary>
    /// Called once when the event is cleared.
    /// </summary>
    public virtual void OnCleared() { }

    /// <summary>
    /// Called every frame to update the event.
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Stops and clears all enemy spawning coroutines.
    /// </summary>
    public void StopAndClearEnemySpawningCoroutines()
    {
        foreach (Coroutine coroutine in enemySpawningCoroutines)
        {
            if (coroutine != null) eventManager.StopCoroutine(coroutine);
        }
        enemySpawningCoroutines.Clear();
    }
}
