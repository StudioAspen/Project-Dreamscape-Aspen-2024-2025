using UnityEngine;

[System.Serializable]
public class PlayerIdleState : PlayerBaseState
{
    public override void OnEnter()
    {
        player.PlayDefaultAnimation();

        player.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        player.AccelerateToHorizontalSpeed(0f);
        player.ApplyHorizontalVelocity();

        if (player.MoveDirection != Vector3.zero)
        {
            player.ChangeState(player.PlayerWalkState);
        }
    }
}
