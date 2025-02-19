using UnityEngine;

[System.Serializable]
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
    /// <returns>Whether the ability was sucessfully activated.</returns>
    public bool TryChangeAbilityState(PlayerAbilityStateSO abilitySO, bool willIgnoreCurrentAbility = false)
    {
        if (abilitySO == null)
        {
            Debug.LogError("Ability is null");
            return false;
        }

        if (!willIgnoreCurrentAbility && !abilitySO.CanUseAbility(player)) return false;

        PlayerAbilityStateSO abilityCopy = abilitySO.CreateRuntimeInstance(player);
        SetAbilityState(abilityCopy);
        player.ChangeState(this, true);

        return true;
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
