using Unity.VisualScripting;
using UnityEngine;

public class EntityBaseStateSO : ScriptableObject
{
    /// <summary>
    /// The entity that this state belongs to.
    /// </summary>
    private protected Entity entity;

    /// <summary>
    /// Clones and creates a runtime instance of the specified type with the given entity.
    /// This is so that each entity has its own instance of the state.
    /// </summary>
    /// <typeparam name="T">The type of the runtime instance.</typeparam>
    /// <param name="originalState">The original state to clone the runtime instance with.</param>
    /// <param name="entity">The entity to initialize the runtime instance with.</param>
    /// <returns>The created runtime instance.</returns>
    public static T CreateRuntimeInstance<T>(EntityBaseStateSO originalState, Entity entity) where T : EntityBaseStateSO
    {
        EntityBaseStateSO runtimeInstance = Instantiate(originalState);
        runtimeInstance.Init(entity);
        return runtimeInstance as T;
    }

    /// <summary>
    /// Creates a runtime instance of the specified type with the given entity.
    /// Any config variables will be default values.
    /// This is so that each entity has its own instance of the state.
    /// </summary>
    /// <typeparam name="T">The type of the runtime instance.</typeparam>
    /// <param name="entity">The entity to initialize the runtime instance with.</param>
    /// <returns>The created runtime instance.</returns>
    public static T CreateRuntimeInstance<T>(Entity entity) where T : EntityBaseStateSO
    {
        EntityBaseStateSO runtimeInstance = ScriptableObject.CreateInstance<T>();
        runtimeInstance.Init(entity);
        return runtimeInstance as T;
    }

    /// <summary>
    /// Initializes the state with the specified entity.
    /// Override this method to initialize the state with derived entities or other components.
    /// </summary>
    /// <param name="entity">The entity to initialize the state with.</param>
    private protected virtual void Init(Entity entity)
    {
        this.entity = entity;
    }

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
