using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Charge Attack Activated")]
public class ChargeAttackActivatedStatusEffectSO : StatusEffectSO
{
    private Weapon hammer;

    [field: Header("Charge Attack Activated: Settings")]
    [field: SerializeField] public ComboDataSO ChargedAttackCombo { get; private set; }
    [field: SerializeField] public float MaxChargeDuration { get; private set; } = 3f;

    /// <summary>
    /// Sets the maximum charge duration for the charge attack activated status effect.
    /// </summary>
    /// <param name="newDuration">The new duration value to set.</param>
    public void SetMaxChargeDuration(float newDuration)
    {
        MaxChargeDuration = newDuration;
    }

    private protected override void OnApply()
    {
        base.OnApply();

        Player player = FindObjectOfType<Player>();
        if(player == null)
        {
            RemoveSelf();
            return;
        }

        hammer = player.GetComponentInChildren<Weapon>();
        if(hammer == null)
        {
            Debug.LogError($"{name}: Hammer not found on entity: {player.name}");
            RemoveSelf(); // If theres no hammer, remove this status effect
            return;
        }

        hammer.AddCombo(ChargedAttackCombo); // Add the charged attack combo to the hammer's list of combos
    }

    public override void Cancel()
    {
        base.Cancel();

        if(hammer != null)
        {
            hammer.RemoveCombo(ChargedAttackCombo);
        }
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
        ChargeAttackActivatedStatusEffectSO overridingStatusEffect = newStatusEffect as ChargeAttackActivatedStatusEffectSO;

        SetMaxChargeDuration(overridingStatusEffect.MaxChargeDuration);
    }
}