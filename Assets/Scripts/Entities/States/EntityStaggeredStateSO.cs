using System.Collections;
using UnityEngine;

public class EntityStaggeredStateSO : EntityBaseStateSO
{
    private protected float timer = 0f;

    public EntityStaggeredStateSO(Entity entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.TransitionToAnimation("Hit");

        timer = 0f;

        entity.SetSpeedModifier(0);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        entity.ApplyGravity();

        timer += entity.LocalDeltaTime;

        if (timer > entity.StaggerDuration)
        {
            entity.ChangeState(entity.DefaultState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        
    }
}
