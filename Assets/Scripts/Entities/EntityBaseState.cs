using Unity.VisualScripting;
using UnityEngine;

public class EntityBaseState
{
    /// <summary>
    /// Called once when entering the state.
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// Called once when exiting the state.
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// Called every frame to update the state.
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// Called every fixed frame rate to update the state.
    /// </summary>
    public virtual void FixedUpdate() { }

    /// <summary>
    /// Called when the character controller hits a collider.
    /// </summary>
    /// <param name="hit">The collision information.</param>
    public virtual void OnControllerColliderHit(ControllerColliderHit hit) { }
}
