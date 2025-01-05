using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Linq;

public class EventManager : MonoBehaviour
{
    private GameManager gameManager;
    private WorldManager worldManager;

    #region Event Machine
    [SerializeField] private List<WorldEventSO> events = new List<WorldEventSO>();

    public WorldEventSO CurrentEvent { get; private set; } // Current event doesn't update if the game state is not PLAYING
    public WorldEventSO DefaultEvent { get; private set; }

    public Dictionary<Type, WorldEventSO> Events { get; private set; } = new Dictionary<Type, WorldEventSO>();

    /// <summary>
    /// Initializes the states for the EventManager state machine.
    /// </summary>
    private void InitializeEvents()
    {
        foreach(WorldEventSO worldEvent in events)
        {
            worldEvent.Init(this, worldManager);
            AddEvent(worldEvent);
        }
    }

    /// <summary>
    /// Adds a state to the dictionary. Prevents duplicate types.
    /// </summary>
    /// <param name="newEvent">The state to add.</param>
    private void AddEvent(WorldEventSO newEvent)
    {
        Type stateType = newEvent.GetType();
        if (Events.ContainsKey(stateType))
        {
            Debug.LogWarning($"State of type {stateType.Name} is already added. Skipping duplicate.");
            return;
        }

        Events[stateType] = newEvent;
    }

    /// <summary>
    /// Gets the state of the specified type from the state machine.
    /// </summary>
    /// <param name="eventType">The type of the state.</param>
    /// <returns>The state of the specified type.</returns>
    public WorldEventSO GetEvent(Type eventType)
    {
        if (!Events.ContainsKey(eventType))
        {
            Debug.LogError($"State of type {eventType.Name} not found in the state machine.");
            return null;
        }

        return Events[eventType];
    }

    /// <summary>
    /// Sets the start state of the EventManager state machine to the specified type.
    /// </summary>
    /// <param name="eventType">The type of the state.</param>
    private void SetStartEvent(Type eventType)
    {
        CurrentEvent = GetEvent(eventType);
        CurrentEvent.OnStarted();
    }

    /// <summary>
    /// Sets the default state of the EventManager state machine to the specified type.
    /// </summary>
    /// <param name="eventType">The type of the state.</param>
    private void SetDefaultEvent(Type eventType)
    {
        DefaultEvent = GetEvent(eventType);
    }

    /// <summary>
    /// Changes the state of the EventManager to the specified type.
    /// Doesn't exit the current state since that is handled when clearing the event.
    /// Changes the game state to PLAYING.
    /// </summary>
    /// <param name="eventType">The type of the state.</param>
    public void ChangeEvent(Type eventType)
    {
        CurrentEvent = GetEvent(eventType);
        CurrentEvent.OnStarted();

        gameManager.ChangeState(GameState.PLAYING);
    }

    /// <summary>
    /// Returns a random event from the available states.
    /// </summary>
    /// <returns>A random event state.</returns>
    public WorldEventSO GetRandomEvent()
    {
        int randomIndex = UnityEngine.Random.Range(0, Events.Count);
        return Events.Values.ToArray()[randomIndex];
    }
    #endregion

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        worldManager = GetComponent<WorldManager>();

        InitializeEvents();
    }

    private void Start()
    {
        SetDefaultEvent(typeof(SurvivalWorldEventSO));
    }

    private void Update()
    {
        if (gameManager.CurrentState != GameState.PLAYING) return;
        
        CurrentEvent?.Update();

        if(Input.GetKeyDown(KeyCode.K))
        {
            ClearEvent();
        }
    }

    /// <summary>
    /// Clears the current event and changes the game state to BIOME_SELECTION.
    /// Unlike a traditional state machine, the EventManager state machine calls OnExit() when clearing the event instead of when changing states.
    /// </summary>
    public void ClearEvent()
    {
        CurrentEvent?.OnCleared();

        gameManager.ChangeState(GameState.BIOME_SELECTION);
    }
}
