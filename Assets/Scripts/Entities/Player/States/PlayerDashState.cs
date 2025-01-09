using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerDashState : PlayerBaseState
{
    [Header("References")]
    [SerializeField] private ParticleSystem dashTrailParticle;

    [field: Header("Config")]
    [field: SerializeField] public float DashDuration { get; private set; } = 0.25f;
    [field: SerializeField] public float InitialDashVelocity { get; private set; } = 75f;
    [field: SerializeField] public float SprintDurationAfterDash { get; private set; } = 2f;
    [field: SerializeField] public float DashCooldown { get; private set; } = 1f;

    private bool isReadyToDash => dashCooldownTimer >= DashCooldown;
    private float dashCooldownTimer = Mathf.Infinity;

    private float timer;
    private float currDashSpeed;
    private float maxSpeed;

    private protected override void Init(Entity entity)
    {
        base.Init(entity);

        // Find the dash trail particle system if it is not set
        if(dashTrailParticle == null) dashTrailParticle = GameObject.Find("Trail").GetComponent<ParticleSystem>();
        if (dashTrailParticle == null) Debug.LogError("Cannot find dash trail");
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Dash");

        dashCooldownTimer = 0f; // Reset the dash cooldown timer when you start dashing
        
        timer = 0f;
        currDashSpeed = InitialDashVelocity;
        maxSpeed = player.MaxSpeed;

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
        timer += player.LocalDeltaTime;

        currDashSpeed = (InitialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(timer / DashDuration - 1, 2))) + maxSpeed;

        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();

        player.InstantlySetHorizontalSpeed(currDashSpeed);
        player.ApplyHorizontalVelocity();

        if (timer > DashDuration)
        {
            if (!player.IsGrounded)
            {
                player.ChangeState(player.PlayerFallState);
            }
            else
            {
                player.PlayerSprintState.SetSprintDuration(SprintDurationAfterDash);
                player.ChangeState(player.PlayerSprintState);
            }  
        }

        dashCooldownTimer = 0; // keeps dash cooldown timer at 0 so that once you stop dashing, the timer goes up
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
        bool isPlayerExceedingMaxSpeed = player.GetHorizontalVelocity().magnitude > player.MaxSpeed;

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

        return true;
    }
}
