using Unity.VisualScripting;
using UnityEngine;

public abstract class EntityBaseState
{
    /// <summary>
    /// The entity that this state belongs to.
    /// Entity is found in the parent object.
    /// </summary>
    private protected Entity entity;

    /// <summary>
    /// Initializes the entity reference.
    /// Override this method to add custom initialization behavior.
    /// </summary>
    public virtual void Init(Entity entity)
    {
        this.entity = entity;
    }

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
    /// Called when the character controller hits a collider.
    /// </summary>
    /// <param name="hit">The collision information.</param>
    public virtual void OnOnControllerColliderHit(ControllerColliderHit hit) { }
}
