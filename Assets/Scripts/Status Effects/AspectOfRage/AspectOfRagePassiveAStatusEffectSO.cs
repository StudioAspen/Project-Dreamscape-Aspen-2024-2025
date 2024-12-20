using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive A")]
public class AspectOfRagePassiveAStatusEffectSO : StatusEffectSO
{
    private Weapon ownerWeapon;

    [field: Header("Aspect of Rage Passive A: Settings")]
    [field: SerializeField] public StatusEffectSO BurningRageStack { get; private set; }
    [field: SerializeField] public StatusEffectSO BurningRageStackExtension { get; private set; }
    private StatusEffectSO currentBurningRageStack;

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        currentBurningRageStack = BurningRageStack;

        ownerWeapon = entity.GetComponentInChildren<Weapon>();
        if(ownerWeapon == null)
        {
            Debug.LogError($"{name}: Weapon not found on entity: {entity.name}");
            return;
        }

        ownerWeapon.OnWeaponHit.AddListener(WeaponStacks_OnWeaponHit);
    }

    public override void Cancel()
    {
        base.Cancel();

        ownerWeapon.OnWeaponHit.RemoveListener(WeaponStacks_OnWeaponHit);
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;
 
        currentBurningRageStack = (newStatusEffect as AspectOfRagePassiveAStatusEffectSO).BurningRageStackExtension;

        return true;
    }

    // for stacks
    private void WeaponStacks_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        EntityStatusEffector.TryApplyStatusEffect(victim.gameObject, currentBurningRageStack, entity.gameObject);
    }
}
