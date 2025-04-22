using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive A")]
public class AspectOfRagePassiveAStatusEffectSO : StatusEffectSO
{
    private Weapon ownerWeapon;

    [field: Header("Aspect of Rage Passive A: Settings")]
    [field: SerializeField] public BurningRageStatusEffectSO BurningRageStack { get; private set; }

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        ownerWeapon = entity.GetComponentInChildren<Weapon>();
        if (ownerWeapon == null)
        {
            Debug.LogError($"{name}: Weapon not found on entity: {entity.name}");
            RemoveSelf(); // If theres no Weapon, remove this passive
            return;
        }

        ownerWeapon.OnWeaponHit += Weapon_OnWeaponHit;
    }

    public override void Cancel()
    {
        base.Cancel();

        ownerWeapon.OnWeaponHit -= Weapon_OnWeaponHit;
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        AspectOfRagePassiveAStatusEffectSO overridingStatusEffect = newStatusEffect as AspectOfRagePassiveAStatusEffectSO;

        BurningRageStack = overridingStatusEffect.BurningRageStack;
    }


    // for stacks
    private void Weapon_OnWeaponHit(Entity attacker, Entity victim, Vector3 hitPoint, int damageValue)
    {
        EntityStatusEffector.TryApplyStatusEffect(victim.gameObject, BurningRageStack, attacker.gameObject);
    }
}
