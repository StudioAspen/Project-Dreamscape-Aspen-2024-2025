using UnityEngine;

public class EntityDeathState : BaseState
{
    private Entity entity;

    public EntityDeathState(Entity entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.PlayAnimation("Death", 0.25f);

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
