using UnityEngine;

public class TemporaryWindWackerStatusEffectSO : DurationStatusEffectSO
{
    [field: Header("Temporary Wind Wacker: Settings")]
    [field: SerializeField] public float WindWackerDamage {get;private set;} = 1f;

    public float RemainingDuration { get; private set; }

    private protected override void OnApply()
    {
        base.OnApply();

        RemainingDuration = Duration;
    }

    public override void Update()
    {
        base.Update();

        RemainingDuration -= Time.deltaTime;

        if (RemainingDuration <= 0)
        {
            OnExpire();
        }
    }

    private protected override void OnExpire()
    {
        base.OnExpire();

        player.DealDamageToOtherEntity(WindWackerDamage);
    }
}