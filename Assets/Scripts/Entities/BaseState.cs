using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseState
{
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
