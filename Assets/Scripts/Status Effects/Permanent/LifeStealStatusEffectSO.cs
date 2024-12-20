using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Life Steal")]
public class LifeStealStatusEffectSO : StatusEffectSO
{
    private Weapon ownerWeapon;
    private ChainingSystem chaining;

    [field: Header("Life Steal: Settings")]
    [field: SerializeField] public float InitialPercent { get; private set; } = 0.0f;
    [field: SerializeField] public float MaxPercent { get; private set; } = 0.2f;
    [field: SerializeField] public float LifestealStep { get; private set; } = 0.05f;

    private protected override void OnApply()
    {
        base.OnApply();

        ownerWeapon = entity.GetComponentInChildren<Weapon>();
        chaining = entity.GetComponent<ChainingSystem>();
        if (ownerWeapon == null)
        {
            Debug.LogError($"{name}: Weapon not found on entity: {entity.name}");
            return;
        }
        if (chaining == null)
        {
            Debug.LogError($"{name}: Chaining system not found on entity: {entity.name}");
            return;
        }

        ownerWeapon.OnWeaponHit.AddListener(Weapon_OnWeaponHit);
    }

    public override void Cancel()
    {
        base.Cancel();

        ownerWeapon.OnWeaponHit.RemoveListener(Weapon_OnWeaponHit);
    }

    public override bool OnStack(StatusEffectSO newStatusEffect)
    {
        if (!base.OnStack(newStatusEffect)) return false;

        // add expansion logic here when stacked

        return true;
    }

    private void Weapon_OnWeaponHit(Entity source, Entity victim, Vector3 hitPoint, int damageValue)
    {
        float lifestealPercent = Mathf.Clamp(InitialPercent + (chaining.ChainCount - 1) * LifestealStep, InitialPercent, MaxPercent);
        int healValue = Mathf.RoundToInt(damageValue * lifestealPercent);

        if (healValue > 0)
        {
            source.Heal(Mathf.RoundToInt(damageValue * lifestealPercent));
        }
    }
}