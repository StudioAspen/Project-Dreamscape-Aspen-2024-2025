using Unity.VisualScripting;
using UnityEngine;

public abstract class EntityBaseState : BaseState
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
    /// Called when the character controller hits a collider.
    /// </summary>
    /// <param name="hit">The collision information.</param>
    public virtual void OnOnControllerColliderHit(ControllerColliderHit hit) { }

    /// <summary>
    /// Called by the entity to draw the gizmos during the state
    /// </summary>
    public virtual void OnDrawGizmos() { }
}
