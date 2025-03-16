using UnityEngine;

public class TemporaryWindWackerStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Temporary Wind Wacker: Settings")]
    [field: SerializeField] public float WindWackerDamage {get;private set;} = 1f;

    private protected override void OnApply()
    {
        base.OnApply();
    }

    public override void Update()
    {
        base.Update();
    }

    private protected override void OnExpire()
    {
        base.OnExpire();

        entity.TakeDamage((int)WindWackerDamage, entity.GetColliderCenterPosition(), source);
    }
}