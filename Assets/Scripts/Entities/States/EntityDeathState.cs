using UnityEngine;

public class EntityDeathState : EntityBaseState
{
    public override void OnEnter()
    {
        entity.TransitionToAnimation("Death");

        entity.SetSpeedModifier(0f);

        entity.SetLocalTimeScale(1f);

        entity.IgnoreOtherEntityCollisions(true);

        entity.DropShards();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();
    }
}
