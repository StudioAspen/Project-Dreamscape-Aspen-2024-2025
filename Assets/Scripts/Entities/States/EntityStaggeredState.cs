using System.Collections;
using UnityEngine;

public class EntityStaggeredState : BaseState
{
    private Entity entity;

    private protected float timer = 0f;

    public EntityStaggeredState(Entity entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.PlayAnimation("Stagger");

        timer = 0f;

        entity.SetSpeedModifier(0);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        timer += Time.deltaTime;

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
