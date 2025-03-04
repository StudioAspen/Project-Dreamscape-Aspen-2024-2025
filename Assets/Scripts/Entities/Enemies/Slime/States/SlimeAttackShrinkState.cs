using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class SlimeAttackShrinkState : SlimeBaseState
{
    [field: SerializeField] public float AttackShrinkDuration { get; private set; } = 1f;
    [field: SerializeField] public Ease ShrinkEase { get; private set; } = Ease.InCubic;

    private float timer;

    public override void OnEnter()
    {
        slime.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit()
    {
        slime.SizeScale.SetBaseValue(slime.IsSmall ? slime.SmallSize : 1f);
    }

    public override void OnUpdate()
    {
        slime.ApplyGravity();

        timer += slime.LocalDeltaTime;
        if(timer > AttackShrinkDuration)
        {
            slime.ChangeState(slime.SlimeWanderState);
            return;
        }

        float parameter = DOVirtual.EasedValue(0f, 1f, timer / AttackShrinkDuration, ShrinkEase);
        float currentSize = slime.IsSmall ? slime.SmallSize : 1f;
        slime.SizeScale.SetBaseValue(Mathf.Lerp(currentSize * slime.SlimeAttackExpandState.AttackExpandSize, currentSize, parameter));
    }
}
