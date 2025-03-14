using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerFallState : PlayerBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(AnimationClip, 0, 0.25f);
    }

    public override void OnExit()
    {
        AkSoundEngine.PostEvent("PlayerLand", player.gameObject);
    }

    public override void OnUpdate()
    {
        player.ApplyGravity();

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
            return;
        }

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToHorizontalSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.SetSpeedModifier(0.25f);
            player.AccelerateToHorizontalSpeed(0f);
        }
            
        player.RotateToTargetRotation(); 
        player.InstantlySetHorizontalSpeed(player.GetHorizontalVelocity().magnitude);
        player.ApplyHorizontalVelocity();
    }
}