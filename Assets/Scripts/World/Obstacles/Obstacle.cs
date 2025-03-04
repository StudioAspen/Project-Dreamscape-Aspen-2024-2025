using System;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    private protected Animator animator;

    #region State Machine
    [field: Header("Obstacle: Base States")]
    [field: SerializeField] public ObstacleBaseState StartState { get; protected set; }
    [field: SerializeField] public ObstacleBaseState DefaultState { get; private set; }
    public ObstacleBaseState CurrentState { get; protected set; }
    public ObstacleBaseState PreviousState { get; protected set; }

    /// <summary>
    /// Change the state machine state to the specified new state.
    /// Force changing is used to change the state even when the current state is the same and is set to false by default.
    /// Override this function if you want to add custom state changing logic.
    /// </summary>
    /// <param name="state">The new state to change to.</param>
    /// <param name="willForceChange">Whether to change the state even when the current state is the same.</param>
    public virtual void ChangeState(ObstacleBaseState state, bool willForceChange = false)
    {
        if (!willForceChange && CurrentState == state) return;

        PreviousState = CurrentState;

        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
    }
    #endregion

    /// <summary>
    /// Action that is invoked when the obstacle takes damage.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>Obstacle damagedObstacle</c>: The damaged obstacle.</description></item>
    /// <item><description><c>Vector3 hitPoint</c>: Where the obstacle was damaged.</description></item>
    /// <item><description><c>GameObject source</c>: The object that damaged the obstacle.</description></item>
    /// </list>
    /// </remarks>
    public Action<Obstacle, Vector3, GameObject> OnDamaged = delegate { };

    private void Awake()
    {
        animator = GetComponent<Animator>();
        OnAwake();
    }

    /// <summary>
    /// This method is called during the Awake phase of the MonoBehaviour lifecycle.
    /// </summary>
    private protected abstract void OnAwake();

    private void Start()
    {
        CurrentState = StartState;
        CurrentState?.OnEnter();
        OnStart();
    }

    /// <summary>
    /// This method is called during the Start phase of the MonoBehaviour lifecycle.
    /// </summary>
    private protected abstract void OnStart();

    private void Update()
    {
        CurrentState?.OnUpdate();
        OnUpdate();
    }

    /// <summary>
    /// This method is called every frame of the MonoBehaviour lifecycle.
    /// </summary>
    private protected abstract void OnUpdate();

    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OnOnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CurrentState.OnOnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        CurrentState.OnOnTriggerExit(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CurrentState.OnOnCollisionEnter(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CurrentState.OnOnCollisionStay(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        CurrentState.OnOnCollisionExit(collision);
    }

    /// <summary>
    /// This method is called when the obstacle takes damage.
    /// </summary>
    /// <param name="source">The object that damaged the obstacle.</param>
    public void TakeDamage(Vector3 hitPoint, GameObject source)
    {
        OnDamaged.Invoke(this, hitPoint, source);
    }

    /// <summary>
    /// Transitions the animator to the specified animation using the specified transition duration.
    /// </summary>
    /// <param name="animation">The name of the animation to transition to.</param>
    /// <param name="transitionDuration">The duration of the transition.</param>
    public void TransitionToAnimation(string animation, float transitionDuration = 0.1f)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration);
    }
}
