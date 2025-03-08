using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shielder Memory Shield Up Ability", menuName = "Memory Abilities/Shielder/Shield Up")]
public class PlayerLeaperShieldUpAbilityStateSO : PlayerAbilityStateSO
{
    [field: Header("Config")]
    public TemporaryUnbreakableShieldStatusEffectSO UnbreakableShieldStatusEffect { get; private set; }
    private TemporaryUnbreakableShieldStatusEffectSO statusEffectInstance;
    [field: SerializeField] public float DurationMultiplier { get; private set; } = 1f;
    public override bool CanUseAbility(Player player)
    {
        bool cannotUseAbility =
            !player.IsGrounded ||
            player.CurrentState == player.PlayerAttackState ||
            player.CurrentState == player.PlayerChargeState ||
            player.CurrentState == player.PlayerDashState;

        return !cannotUseAbility;
    }

    public override bool CanCancelAbility(Player player, EntityBaseState desiredState)
    {
        return desiredState == player.PlayerAbilityState || desiredState == player.DefaultState;
    }

    public override void OnEnter()
    {
        // Apply the unbreakable shield status effect to the player
        statusEffectInstance = Instantiate(UnbreakableShieldStatusEffect);
        statusEffectInstance.OverrideDuration(statusEffectInstance.Duration * DurationMultiplier);
        statusEffectInstance.Init(player.GetComponent<EntityStatusEffector>(), player.gameObject);
    }

    public override void OnExit()
    {
        if (statusEffectInstance != null)
        {
            statusEffectInstance.Cancel();
            statusEffectInstance = null;
        }
    }

    public override void OnUpdate()
    {
    }


}
