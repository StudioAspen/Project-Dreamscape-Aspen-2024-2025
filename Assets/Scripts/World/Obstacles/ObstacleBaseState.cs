using UnityEngine;

public abstract class ObstacleBaseState : MonoBehaviour
{
    private protected Obstacle obstacle;

    private void Awake()
    {
        obstacle = GetComponent<Obstacle>();
        Init();
    }

    /// <summary>
    /// Called once when instantiated. Equivalent to Awake()
    /// </summary>
    private protected abstract void Init();

    /// <summary>
    /// Called once when entering the state.
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// Called once when exiting the state.
    /// </summary>
    public abstract void OnExit();

    /// <summary>
    /// Called every frame to update the state.
    /// </summary>
    public abstract void OnUpdate();

    /// <summary>
    /// Mimics the monobehaviour method, but only runs in its current state
    /// </summary>
    public virtual void OnOnTriggerEnter(Collider other) { }

    /// <summary>
    /// Mimics the monobehaviour method, but only runs in its current state
    /// </summary>
    public virtual void OnOnTriggerStay(Collider other) { }

    /// <summary>
    /// Mimics the monobehaviour method, but only runs in its current state
    /// </summary>
    public virtual void OnOnTriggerExit(Collider other) { }

    /// <summary>
    /// Mimics the monobehaviour method, but only runs in its current state
    /// </summary>
    public virtual void OnOnCollisionEnter(Collision collision) { }

    /// <summary>
    /// Mimics the monobehaviour method, but only runs in its current state
    /// </summary>
    public virtual void OnOnCollisionStay(Collision collision) { }

    /// <summary>
    /// Mimics the monobehaviour method, but only runs in its current state
    /// </summary>
    public virtual void OnOnCollisionExit(Collision collision) { }
}
