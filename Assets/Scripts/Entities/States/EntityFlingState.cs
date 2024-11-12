using UnityEngine;

public class EntityFlingState : BaseState
{
    private Entity entity;

    private protected float timer;
    private protected float stunDuration;

    private protected Vector3 direction;
    private protected float force;

    private protected bool touchedGround;

    public EntityFlingState(Entity entity)
    {
        this.entity = entity;
    }

    public virtual void SetFlingSettings(Vector3 direction, float force, float stunDuration)
    {
        this.direction = direction;
        this.force = force;
        this.stunDuration = stunDuration;
    }

    public override void OnEnter()
    {
        entity.DefaultTransitionToAnimation("Falling");

        timer = 0f;
        touchedGround = false;

        entity.SetSpeedModifier(0);

        entity.Fling(direction, force, stunDuration);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer > stunDuration)
        {
            entity.ChangeState(entity.DefaultState);
            return;
        }

        if (entity.IsGrounded && !touchedGround)
        {
            touchedGround = true;
            Debug.Log("Flinged touched the ground");
        }
    }

    public override void FixedUpdate()
    {

    }
}
