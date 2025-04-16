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
    [SerializeField] private TutorialWorldEventSO tutorialEvent;
    [SerializeField] private List<WorldEventSO> events = new List<WorldEventSO>();
    public Dictionary<Type, WorldEventSO> EventsDictionary { get; private set; } = new Dictionary<Type, WorldEventSO>();

    public WorldEventSO CurrentEvent { get; private set; } // Current event doesn't update if the game state is not PLAYING
    public WorldEventSO DefaultEvent { get; private set; }

    /// <summary>
    /// Action that is invoked when the event changes.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>WorldEventSO newEvent</c>: The new event</description></item>
    /// </list>
    /// </remarks>
    public Action<WorldEventSO> OnEventChanged = delegate { };


    /// <summary>
    /// Initializes the states for the EventManager state machine.
    /// </summary>
    private void InitializeEvents()
    {
        tutorialEvent.Init(this, worldManager); // Initialize the tutorial event
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
        if (EventsDictionary.ContainsKey(stateType))
        {
            Debug.LogWarning($"State of type {stateType.Name} is already added. Skipping duplicate.");
            return;
        }

        EventsDictionary[stateType] = newEvent;
    }

    /// <summary>
    /// Gets the state of the specified type from the state machine.
    /// </summary>
    /// <param name="eventType">The type of the state.</param>
    /// <returns>The state of the specified type.</returns>
    public WorldEventSO GetEvent(Type eventType)
    {
        if (!EventsDictionary.ContainsKey(eventType))
        {
            Debug.LogError($"State of type {eventType.Name} not found in the state machine.");
            return null;
        }

        return EventsDictionary[eventType];
    }

    /// <summary>
    /// Gets the state of the specified type from the state machine.
    /// Casts the state to the specified type.
    /// </summary>
    /// <returns>The state of the specified type.</returns>
    public T GetEvent<T>() where T : WorldEventSO
    {
        return GetEvent(typeof(T)) as T;
    }

    /// <summary>
    /// Sets the start state of the EventManager state machine to the specified type.
    /// </summary>
    /// <param name="eventType">The type of the state.</param>
    private void SetStartEvent(Type eventType)
    {
        CurrentEvent = GetEvent(eventType);
        CurrentEvent.Start();
    }

    /// <summary>
    /// Sets the start state of the EventManager state machine to the specified type.
    /// </summary>
    private void SetStartEvent<T>() where T : WorldEventSO
    {
        SetStartEvent(typeof(T));
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
    /// Sets the default state of the EventManager state machine to the specified type.
    /// </summary>
    private void SetDefaultEvent<T>() where T : WorldEventSO
    {
        SetDefaultEvent(typeof(T));
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
        CurrentEvent.Start();
        OnEventChanged?.Invoke(CurrentEvent);

        gameManager.ChangeState(GameState.PLAYING);
    }

    /// <summary>
    /// Returns a random event from the available states.
    /// </summary>
    /// <returns>A random event state.</returns>
    public WorldEventSO GetRandomEvent()
    {
        int randomIndex = UnityEngine.Random.Range(0, EventsDictionary.Count);
        return EventsDictionary.Values.ToArray()[randomIndex];
    }
    #endregion

    private void Awake()
    {
        worldManager = GetComponent<WorldManager>();
        InitializeEvents();

        Player.OnPlayerLoaded += Player_OnPlayerLoaded;
        Player.OnPlayerDestroyed += Player_OnPlayerDestroyed;
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        SetDefaultEvent<SurvivalWorldEventSO>();
    }

    private void OnDestroy()
    {
        Player.OnPlayerLoaded -= Player_OnPlayerLoaded;
        Player.OnPlayerDestroyed -= Player_OnPlayerDestroyed;
    }

    private void Player_OnPlayerLoaded(Player player)
    {
        Player.OnPlayerLoaded -= Player_OnPlayerLoaded;

        StartTutorialEvent();
    }

    private void Player_OnPlayerDestroyed(Player player)
    {
        // If the player is destroyed, fail the event. Keeps track of players before failing if this is multiplayer.

        FailEvent();
    }

    private void Update()
    {
        if (gameManager.CurrentState != GameState.PLAYING) return;
        
        CurrentEvent?.Update();

        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.LogWarning("Cheat: Insta clearing event");
            ClearEvent();
        }
    }

    /// <summary>
    /// Clears the current event and changes the game state to BIOME_SELECTION.
    /// Unlike a traditional state machine, the EventManager state machine calls OnExit() when clearing the event instead of when changing states.
    /// </summary>
    public void ClearEvent()
    {
        CurrentEvent?.Clear();

        gameManager.ChangeState(GameState.ASPECT_SELECTION);
    }

    /// <summary>
    /// Clears the current event and changes the game state to GAME_OVER.
    /// Unlike a traditional state machine, the EventManager state machine calls OnExit() when clearing the event instead of when changing states.
    /// </summary>
    public void FailEvent()
    {
        CurrentEvent?.Clear();

        gameManager.ChangeState(GameState.GAME_OVER);
    }

    /// <summary>
    /// Starts the tutorial event and changes the game state to PLAYING.
    /// </summary>
    public void StartTutorialEvent()
    {
        CurrentEvent = tutorialEvent;
        CurrentEvent.Start();
    }
}
