using UnityEngine;

[System.Serializable]
public class PlayerWalkState : PlayerBaseState
{
    public override void OnEnter()
    {
        player.PlayDefaultAnimation();

        player.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState);
            return;
        }

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToHorizontalSpeed(player.MovementSpeed);
        player.ApplyHorizontalVelocity();
    }
}
