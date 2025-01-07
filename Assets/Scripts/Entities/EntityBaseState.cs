using Unity.VisualScripting;
using UnityEngine;

public class EntityBaseState : BaseState
{
    public override void OnEnter() { }

    public override void OnExit() { }

    public override void Update() { }

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
