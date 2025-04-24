using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Ambition/Passive A")]
public class AspectofAmbitionPassiveAStatusEffectSO: StatusEffectSO
{
    [field: Header("Aspect of Ambition Passive A: Settings")]
    public float speedBuff { get; protected set; }
    public AmbitionDashState ambitionDashState;

    private void OnValidate()
    {
    }

    private protected override void OnApply()
    {
        base.OnApply();

        Player player = entity as Player;

        // Replace the existing PlayerDashState with ambitionDashState using ReplaceExistingState
        player.ReplaceExistingState(player.PlayerDashState, ambitionDashState);

        // Check if the ambition dash state dashed through an enemy
        if (ambitionDashState.dashedThroughEnemy)
        {
            entity.StatusSpeedModifier.AddMultiplier(speedBuff, this);
        }
    }

    public override void Cancel()
    {
        base.Cancel();
    }

    private protected override void OnStack(StatusEffectSO newStatusEffect)
    {
    }
}