using Unity.VisualScripting;
using UnityEngine;

public class EntityBaseState : MonoBehaviour
{
    /// <summary>
    /// The entity that this state belongs to.
    /// Entity is found in the parent object.
    /// </summary>
    private protected Entity entity;

    /// <summary>
    /// Initializes or creates an instance of the specified state type for the given entity.
    /// If the state already exists, it is returned. If not, a new instance is created and added to the "States" GameObject.
    /// Replaces the base state if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the state.</typeparam>
    /// <param name="entity">The entity that the state belongs to.</param>
    /// <returns>The initialized or created state instance.</returns>
    public static T InitializeOrCreate<T>(Entity entity) where T : EntityBaseState
    {
        // Find the "States" child GameObject in the entity.
        Transform statesTransform = entity.transform.Find("States");
        if (statesTransform == null)
        {
            // If "States" GameObject doesn't exist, create it.
            //Debug.LogWarning($"States GameObject not found in {entity.name}. Please create one. Creating one now to avoid other errors.");

            GameObject statesObject = new GameObject("States");
            statesObject.transform.SetParent(entity.transform);
            statesTransform = statesObject.transform;
        }

        // Check if a state of type T already exists.
        T existingState = statesTransform.GetComponent<T>();
        if (existingState != null)
        {
            // If the state exists, return it.
            //Debug.Log($"{entity.name}: {typeof(T).ToString()} as {existingState.GetType().ToString()} exists.");

            existingState.Init(entity);
            return existingState;
        }

        // Check if a base state exists for T and remove it if found.
        var baseStateType = typeof(T).BaseType;
        if (baseStateType != null && !baseStateType.ToString().EndsWith("BaseState"))
        {
            var baseState = statesTransform.GetComponent(baseStateType);
            if (baseState != null)
            {
                // Remove the base state.
                //Debug.Log($"{entity.name}: Base state, {baseStateType.ToString()}, of {typeof(T).ToString()} exists, removing {baseStateType.ToString()}.");

                GameObject.Destroy(baseState);
            }
        }

        // Add the new state of type T to the "States" GameObject.
        T newState = statesTransform.gameObject.AddComponent<T>();
        newState.Init(entity);

        //Debug.Log($"{entity.name}: Adding {newState.GetType().ToString()}.");

        // Return the newly created state.
        return newState;
    }

    /// <summary>
    /// Initializes the entity reference.
    /// Override this method to add custom initialization behavior.
    /// </summary>
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
    public virtual void OnUpdate() { }

    /// <summary>
    /// Called when the character controller hits a collider.
    /// </summary>
    /// <param name="hit">The collision information.</param>
    public virtual void OnOnControllerColliderHit(ControllerColliderHit hit) { }
}
