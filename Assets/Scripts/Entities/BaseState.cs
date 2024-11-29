using Unity.VisualScripting;
using UnityEngine;

public class BaseState
{
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnControllerColliderHit(ControllerColliderHit hit) { }
}
