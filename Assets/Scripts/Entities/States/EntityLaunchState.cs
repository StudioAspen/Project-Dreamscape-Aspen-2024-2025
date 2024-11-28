using UnityEngine;

public class EntityLaunchState : BaseState
{
    private Entity entity;

    private protected float timer;
    private protected float stunDuration;

    private protected Vector3 direction;
    private protected float force;

    private protected bool touchedGround;

    public EntityLaunchState(Entity entity)
    {
        this.entity = entity;
    }

    public virtual void SetLaunchSettings(Vector3 direction, float force, float stunDuration)
    {
        this.direction = direction;
        this.force = force;
        this.stunDuration = stunDuration;
    }

    public override void OnEnter()
    {
        entity.TransitionToAnimation("FlatFall");

        timer = 0f;
        touchedGround = false;

        entity.SetSpeedModifier(0);

        entity.Launch(direction, force);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += entity.LocalDeltaTime;

        if (timer > stunDuration)
        {
            entity.ChangeState(entity.DefaultState);
            return;
        }

        if (timer > 0.05f && entity.IsGrounded && !touchedGround)
        {
            touchedGround = true;
            entity.TransitionToAnimation("FlatFallImpact");
        }
    }

    public override void FixedUpdate()
    {

    }
}
