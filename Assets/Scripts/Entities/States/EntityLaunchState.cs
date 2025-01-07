using UnityEngine;

public class EntityLaunchState : EntityBaseState
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
        entity.ApplyGravity();

        entity.ApplyHorizontalVelocity();

        timer += entity.LocalDeltaTime;

        if (timer > stunDuration)
        {
            entity.ChangeState(entity.DefaultState);
            return;
        }

        if (entity.IsGrounded && !touchedGround)
        {
            touchedGround = true;

            entity.SetVelocity(Vector3.zero);

            entity.TransitionToAnimation("FlatFallImpact");
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.layer != LayerMask.NameToLayer("Ground")) return;

        Vector3 bounceVelocity = Vector3.Reflect(entity.GetHorizontalVelocity(), hit.normal);
        bounceVelocity.y = entity.Velocity.y;

        entity.SetVelocity(bounceVelocity);
    }
}