using UnityEngine;

public class PlayerAbilityState : PlayerBaseState
{
    public PlayerAbilityStateSO AbilityState { get; private set; }

    public void SetAbilityState(PlayerAbilityStateSO playerAbility)
    {
        AbilityState = playerAbility;
    }

    /// <summary>
    /// Determines if the player can cancel the ability.
    /// Override this method to restrict the ability based on custom conditions.
    /// </summary>
    public bool CanCancelAbility(EntityBaseState desiredState)
    {
        if(AbilityState == null)
        {
            return true;
        }

        return AbilityState.CanCancelAbility(player, desiredState);
    }

    /// <summary>
    /// Changes the ability state of the player.
    /// </summary>
    /// <param name="abilitySO">The ability state to change to.</param>
    /// <param name="willIgnoreCurrentAbility">Determines if the current ability should be ignored.</param>
    public void ChangeAbilityState(PlayerAbilityStateSO abilitySO, bool willIgnoreCurrentAbility = false)
    {
        if (abilitySO == null)
        {
            Debug.LogError("Ability is null");
            return;
        }

        if (!willIgnoreCurrentAbility && !abilitySO.CanUseAbility(player)) return;

        PlayerAbilityStateSO abilityCopy = abilitySO.CreateRuntimeInstance(player);
        SetAbilityState(abilityCopy);
        player.ChangeState(this, true);
    }

    public void SetAbilityAnimationSpeed(float speed)
    {
        player.GetComponent<Animator>().SetFloat("AbilityAnimationSpeed", speed);
    }

    public override void OnEnter()
    {
        AbilityState?.OnEnter();
    }

    public override void OnExit()
    {
        AbilityState?.OnExit();
    }

    public override void OnUpdate()
    {
        AbilityState?.OnUpdate();
    }

    public override void OnOnControllerColliderHit(ControllerColliderHit hit)
    {
        AbilityState?.OnOnControllerColliderHit(hit);
    }
}
