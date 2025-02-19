public abstract class BaseState
{
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
}
