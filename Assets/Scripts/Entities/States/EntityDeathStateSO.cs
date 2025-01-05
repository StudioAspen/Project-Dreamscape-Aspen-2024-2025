using UnityEngine;

public class EntityDeathStateSO : EntityBaseStateSO
{
    public EntityDeathStateSO(Entity entity)
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
