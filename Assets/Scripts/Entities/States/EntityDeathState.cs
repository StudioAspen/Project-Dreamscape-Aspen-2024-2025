using UnityEngine;

public class EntityDeathState : EntityBaseState
{
    public override void OnEnter()
    {
        entity.TransitionToAnimation("Death");

        entity.SetSpeedModifier(0f);

        entity.LocalTimeScale.ClearMultipliers();

        entity.IgnoreOtherEntityCollisions(true);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();
    }
}
