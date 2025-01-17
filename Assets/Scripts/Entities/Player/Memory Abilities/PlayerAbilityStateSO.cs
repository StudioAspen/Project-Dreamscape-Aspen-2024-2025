using UnityEngine;

public class PlayerAbilityStateSO : ScriptableObject
{
    private protected Player player;

    /// <summary>
    /// Creates a runtime instance of the PlayerAbilityStateSO class, passing in the player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The created runtime instance.</returns>
    public PlayerAbilityStateSO CreateRuntimeInstance(Player player)
    {
        PlayerAbilityStateSO runtimeInstance = Instantiate(this);
        runtimeInstance.Init(player);

        return runtimeInstance;
    }

    /// <summary>
    /// Initializes the player ability state with the specified player.
    /// Override this method to add custom initialization logic.
    /// Equivalent to an Awake() method.
    /// </summary>
    /// <param name="player">The player.</param>
    public virtual void Init(Player player)
    {
        this.player = player;
    }

    /// <summary>
    /// Determines if the player can use the ability.
    /// Override this method to restrict the ability based on custom conditions.
    /// </summary>
    public virtual bool CanUseAbility(Player player)
    {
        return true;
    }

    /// <summary>
    /// Determines if the player can cancel the ability.
    /// Override this method to restrict the ability based on custom conditions.
    /// </summary>
    public virtual bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return true;
    }

    /// <summary>
    /// Transitions the player to the specified ability animation.
    /// </summary>
    /// <param name="animationClip">The animation clip to transition to.</param>
    /// <param name="clipDuration">The duration to force the clip to.</param>
    /// <param name="fadeDuration">The duration of the fade transition (optional, default is 0.1 seconds).</param>
    public void TransitionToAbilityAnimation(AnimationClip animationClip, float clipDuration = 0f, float fadeDuration = 0.1f)
    {
        // Calculate the speed based on the clip duration
        float speed = (clipDuration == 0f) ? 1f : animationClip.length / clipDuration;
        player.PlayerAbilityState.SetAbilityAnimationSpeed(speed);

        player.ReplaceOneShotAnimationClip(animationClip, "AbilityPlaceholder");
        player.TransitionToAnimation("Ability", fadeDuration);
    }

    /// <summary>
    /// Called once when entering the state.
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// Called once when exiting the state.
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// Called every frame to update the state.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Called when the character controller hits a collider.
    /// </summary>
    /// <param name="hit">The collision information.</param>
    public virtual void OnOnControllerColliderHit(ControllerColliderHit hit) { }
}
