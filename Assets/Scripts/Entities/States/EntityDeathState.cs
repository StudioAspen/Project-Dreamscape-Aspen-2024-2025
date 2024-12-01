using UnityEngine;

public class EntityDeathState : EntityBaseState
{
    private Entity entity;

    public EntityDeathState(Entity entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.TransitionToAnimation("Death");

        entity.SetSpeedModifier(0f);

        entity.SetLocalTimeScale(1f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        entity.ApplyGravity();
    }

    public override void FixedUpdate()
    {
        
    }
}
