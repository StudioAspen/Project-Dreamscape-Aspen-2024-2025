using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class PlayerDashState : PlayerBaseState
{
    [Header("References")]
    [SerializeField] private ParticleSystem dashTrailParticle;

    [field: Header("Config")]
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float DashDuration { get; private set; } = 0.25f;
    [field: SerializeField] public float InitialDashVelocity { get; private set; } = 75f;
    [field: SerializeField] public float DashCooldown { get; private set; } = 1f;

    private bool isReadyToDash => dashCooldownTimer >= DashCooldown;
    private float dashCooldownTimer = Mathf.Infinity;

    private float timer;
    private float currDashSpeed;
    private float maxSpeed;

    public override void Init(Entity entity)
    {
        base.Init(entity);

        InitializeDashTrail();
    }

    public override void OnEnter()
    {
        player.PlayOneShotAnimation(AnimationClip, DashDuration);
        AkSoundEngine.PostEvent("PlayerDash", player.gameObject);

        dashCooldownTimer = 0f; // Reset the dash cooldown timer when you start dashing
        
        timer = 0f;
        currDashSpeed = InitialDashVelocity;
        maxSpeed = player.BaseSpeed * player.StatusSpeedModifier.GetFloatValue();

        player.ApplyRotationToNextMovement();

        player.InstantlySetHorizontalSpeed(InitialDashVelocity);
    }

    public override void OnExit()
    {
        player.InstantlySetHorizontalSpeed(maxSpeed);
        player.ResetYVelocity();
    }

    public override void OnUpdate()
    {
        if (timer > DashDuration)
        {
            if (!player.IsGrounded)
            {
                player.ChangeState(player.PlayerFallState);
                return;
            }
            else
            {
                player.ChangeState(player.PlayerWalkState);
                return;
            }
        }

        timer += player.LocalDeltaTime;

        currDashSpeed = (InitialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(timer / DashDuration - 1, 2))) + maxSpeed;

        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();

        player.InstantlySetHorizontalSpeed(currDashSpeed);
        player.ApplyHorizontalVelocity();

        dashCooldownTimer = 0; // keeps dash cooldown timer at 0 so that once you stop dashing, the timer goes up
    }

    /// <summary>
    /// Initializes the dash trail particle system by finding the dash trail GameObject and getting the ParticleSystem component.
    /// If the dash trail particle system is not found, it logs an error message.
    /// </summary>
    private void InitializeDashTrail()
    {
        if(dashTrailParticle != null)
        {
            dashTrailParticle.Stop();
            return;
        }

        // Find the dash trail particle system if it is not set
        dashTrailParticle = GameObject.Find("Trail").GetComponent<ParticleSystem>();
        if(dashTrailParticle == null)
        {
            Debug.LogError("Cannot find player dash trail");
        }
    }

    /// <summary>
    /// Handles the cooldown timer for the player's dash ability by incrementing the timer by the player's local delta time.
    /// </summary>
    public void HandleDashCooldown()
    {
        if (dashCooldownTimer < DashCooldown)
        {
            dashCooldownTimer += player.LocalDeltaTime;
        }
    }

    private bool isTrailPreviouslyPlaying;
    /// <summary>
    /// Handles the dash trail effect based on the player's speed.
    /// </summary>
    public void HandleDashTrail()
    {
        bool isPlayerExceedingMaxSpeed = player.GetHorizontalVelocity().magnitude > player.BaseSpeed;

        if(isPlayerExceedingMaxSpeed != isTrailPreviouslyPlaying)
        {
            isTrailPreviouslyPlaying = isPlayerExceedingMaxSpeed;

            if(isPlayerExceedingMaxSpeed)
            {
                dashTrailParticle.Play();
            }
            else
            {
                dashTrailParticle.Stop();
            }
        }
    }

    /// <summary>
    /// Determines whether the player can perform a dash.
    /// </summary>
    /// <returns>True if the player can dash, false otherwise.</returns>
    public bool CanDash()
    {
        if (!isReadyToDash) return false;
        if (player.CurrentState == player.PlayerChargeState) return false;
        if (player.CurrentState == player.PlayerDashState) return false;
        if (player.CurrentState == player.EntityStaggeredState) return false;
        if (player.CurrentState == player.EntityLaunchState) return false;
        if (player.CurrentState == player.EntityStunnedState) return false;

        return true;
    }
}
