using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Rage/Passive A")]
public class AspectOfRagePassiveAStatusEffectSO : StatusEffectSO
{
    private Weapon ownerWeapon;

    [field: Header("Aspect of Rage Passive A: Settings")]
    [field: SerializeField] public float AOEExplosionRadius { get; private set; } = 2.0f;
    [field: SerializeField] public int AOEDamage { get; private set; } = 2;
    [field: SerializeField] public StatusEffectSO BurningRageStack { get; private set; }

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();

        ownerWeapon = entity.GetComponentInChildren<Weapon>();
        if(ownerWeapon == null)
        {
            Debug.LogError($"{name}: Weapon not found on entity: {entity.name}");
            return;
        }

        //ownerWeapon.OnWeaponHit.AddListener(WeaponExplosion_OnWeaponHit);
        ownerWeapon.OnWeaponHit.AddListener(WeaponStacks_OnWeaponHit);
    }

    public override void Cancel()
    {
        base.Cancel();

        //ownerWeapon.OnWeaponHit.RemoveListener(WeaponExplosion_OnWeaponHit);
        ownerWeapon.OnWeaponHit.RemoveListener(WeaponStacks_OnWeaponHit);
    }

    public override bool Override(StatusEffectSO newStatusEffect)
    {
        if (!base.Override(newStatusEffect)) return false;

        // add expansion logic here when stacked

        return true;
    }

    // for stacks
    private void WeaponStacks_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        EntityStatusEffector statusEffector = victim.GetComponent<EntityStatusEffector>();
        if (statusEffector == null) { return; }
        statusEffector.ApplyStatusEffect(BurningRageStack, victim.gameObject);
    }
}
