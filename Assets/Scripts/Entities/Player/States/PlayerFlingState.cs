using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerFlingState : EntityFlingState
{
    private Player player;

    public PlayerFlingState(Player entity) : base(entity)
    {
        player = entity;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer > stunDuration)
        {
            player.ChangeState(player.DefaultState);
            return;
        }

        if (player.IsGrounded && !touchedGround)
        {
            touchedGround = true;
            player.SetVelocity(Vector3.zero);
        }

        player.ApplyGravity();

        player.GroundedMove();
    }

    public override void FixedUpdate()
    {

    }
}
