using UnityEngine;

public class EntityLaunchState : EntityBaseState
{
    [field: SerializeField] public AnimationClip LaunchAnimationClip { get; private set; }
    [field: SerializeField] public AnimationClip GroundImpactAnimationClip { get; private set; }

    private protected float timer;
    private protected float stunDuration;

    private protected Vector3 direction;
    private protected float force;

    private protected bool touchedGround;

    public virtual void SetLaunchSettings(Vector3 direction, float force, float stunDuration)
    {
        this.direction = direction;
        this.force = force;
        this.stunDuration = stunDuration;
    }

    public override void OnEnter()
    {
        entity.PlayOneShotAnimation(LaunchAnimationClip);

        timer = 0f;
        touchedGround = false;

        entity.SetSpeedModifier(0);

        entity.Launch(direction, force);

        entity.IgnoreOtherEntityCollisions();
    }

    public override void OnExit()
    {
        entity.IgnoreOtherEntityCollisions(false);
    }

    public override void OnUpdate()
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

            entity.PlayOneShotAnimation(GroundImpactAnimationClip);
        }
    }

    public override void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.layer != LayerMask.NameToLayer("Ground")) return;

        Vector3 bounceVelocity = Vector3.Reflect(entity.GetHorizontalVelocity(), hit.normal);
        bounceVelocity.y = entity.Velocity.y;

        entity.SetVelocity(bounceVelocity);
    }
}