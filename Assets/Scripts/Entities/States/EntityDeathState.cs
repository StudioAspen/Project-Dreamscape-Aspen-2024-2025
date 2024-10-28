using UnityEngine;

public class EntityDeathState : BaseState
{
    private Entity entity;

    public EntityDeathState(Entity entity) : base(entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.DefaultTransitionToAnimation("Death");

        entity.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }
}
