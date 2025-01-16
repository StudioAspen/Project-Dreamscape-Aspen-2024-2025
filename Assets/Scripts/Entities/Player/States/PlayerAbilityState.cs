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

        return AbilityState.CanCancelAbility(desiredState);
    }

    public override void OnEnter()
    {
        AbilityState?.OnEnter();
    }

    public override void OnExit()
    {
        AbilityState?.OnExit();
        AbilityState = null;
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
