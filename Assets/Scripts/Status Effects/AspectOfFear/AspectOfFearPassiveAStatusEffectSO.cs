using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Aspect of Fear Passive A", menuName = "Status Effect/Aspects/Aspect of Fear/Passive A")]
public class AspectOfFearPassiveAStatusEffectSO : StatusEffectSO
{
    private PlayerCombat playerCombat;

    [field: Header("Aspect of Fear Passive A: Settings")]
    [field: SerializeField] public BurningRageStatusEffectSO BurningRageStack { get; private set; }

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        playerCombat = entity.GetComponent<PlayerCombat>();
        if (playerCombat == null)
        {
            Debug.LogError($"{name}: Player combat not found on entity: {entity.name}");
            RemoveSelf(); // If theres no playerCombat, remove this passive
            return;
        }

        playerCombat.OnAttackInputSwitched += PlayerCombat_OnAttackInputSwitched;
    }

    public override void Cancel()
    {
        base.Cancel();

        playerCombat.OnAttackInputSwitched -= PlayerCombat_OnAttackInputSwitched;
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        AspectOfFearPassiveAStatusEffectSO overridingStatusEffect = newStatusEffect as AspectOfFearPassiveAStatusEffectSO;

        BurningRageStack = overridingStatusEffect.BurningRageStack;
    }

    private void PlayerCombat_OnAttackInputSwitched(ComboAction previousAttackAction, ComboAction newAttackAction)
    {
        Debug.Log($"Detected attack action switch from {previousAttackAction} to {newAttackAction}");

        // Launch a piercing skull that moves forward from where the player is facing
        // and applies a skull debuff to any enemies hit by the skull
    }
}