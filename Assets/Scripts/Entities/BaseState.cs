public class BaseState
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
}
