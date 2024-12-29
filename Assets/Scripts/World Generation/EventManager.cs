using System.Collections.Generic;
using TMPro;
using KBCore.Refs;
using UnityEngine;
using Unity.VisualScripting;
using System;
using System.Linq;

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
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Scene] private WorldManager worldManager;

    #region State Machine
    public BaseState CurrentState { get; private set; }
    public BaseState DefaultState { get; private set; }

    public Dictionary<Type, BaseState> States { get; private set; } = new Dictionary<Type, BaseState>();

    /// <summary>
    /// Initializes the states for the EventManager state machine.
    /// </summary>
    private void InitializeStates()
    {
        AddState(new SurvivalWorldEvent(this, worldManager));
        AddState(new ZonesWorldEvent(this, worldManager));
        AddState(new PrioritiesWorldEvent(this, worldManager));
        AddState(new EscortWorldEvent(this, worldManager));
        AddState(new VisitAllWorldEvent(this, worldManager));
        AddState(new DefendWorldEvent(this, worldManager));
    }

    /// <summary>
    /// Adds a state to the dictionary. Prevents duplicate types.
    /// </summary>
    /// <param name="newState">The state to add.</param>
    private void AddState(BaseState newState)
    {
        Type stateType = newState.GetType();
        if (States.ContainsKey(stateType))
        {
            Debug.LogWarning($"State of type {stateType.Name} is already added. Skipping duplicate.");
            return;
        }

        States[stateType] = newState;
    }

    /// <summary>
    /// Gets the state of the specified type from the state machine.
    /// </summary>
    /// <typeparam name="T">The type of the state.</typeparam>
    /// <returns>The state of the specified type.</returns>
    public BaseState GetState<T>() where T : BaseState
    {
        if (!States.ContainsKey(typeof(T)))
        {
            Debug.LogError($"State of type {typeof(T).Name} not found in the state machine.");
            return null;
        }

        return States[typeof(T)];
    }

    /// <summary>
    /// Sets the start state of the EventManager state machine to the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the state to set as the start state.</typeparam>
    private void SetStartState<T>() where T : BaseState
    {
        CurrentState = GetState<T>();
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Sets the default state of the EventManager state machine to the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the state to set as the default state.</typeparam>
    private void SetDefaultState<T>() where T : BaseState
    {
        DefaultState = GetState<T>();
    }

    /// <summary>
    /// Changes the state of the EventManager to the specified type.
    /// Doesn't change the state if the current state is the same as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the state to change to.</typeparam>
    public void ChangeState<T>() where T : BaseState
    {
        if (CurrentState.GetType() == typeof(T)) return;

        CurrentState.OnExit();
        CurrentState = GetState<T>();
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Force changes the state of the EventManager to the specified type without considering CurrentState.
    /// </summary>
    /// <typeparam name="T">The type of the state to change to.</typeparam>
    public void ForceChangeState<T>() where T : BaseState
    {
        CurrentState.OnExit();
        CurrentState = GetState<T>();
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Returns a random event from the available states.
    /// </summary>
    /// <returns>A random event state.</returns>
    public BaseState GetRandomState()
    {
        int randomIndex = UnityEngine.Random.Range(0, States.Count);
        return States.Values.ToArray()[randomIndex];
    }
    #endregion

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        InitializeStates();
    }

    private void Start()
    {
        SetDefaultState<SurvivalWorldEvent>();
        SetStartState<SurvivalWorldEvent>();
    }

    private void Update()
    {
        CurrentState?.Update();
    }
}