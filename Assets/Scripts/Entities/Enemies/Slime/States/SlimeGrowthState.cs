using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlimeGrowthState : SlimeBaseState
{
    [field: SerializeField] public float GrowthDuration { get; private set; } = 5f;
    [field: SerializeField] public float NoDamageTakenTargetDuration { get; private set; } = 10f;
    [field: SerializeField] public Ease GrowthEase { get; private set; } = Ease.OutCubic;

    private float currentTime;

    public override void OnEnter()
    {
        currentTime = 0f;
        slime.SetSmall(true);
    }

    public override void OnExit()
    {
        slime.SetSmall(false);
        slime.HealToFull(true);
    }

    public override void OnUpdate()
    {
        slime.ApplyGravity();

        currentTime += slime.LocalDeltaTime;
        if(currentTime > GrowthDuration)
        {
            slime.ChangeState(slime.SlimeWanderState);
            return;
        }

        float parameter = DOVirtual.EasedValue(0f, 1f, currentTime / GrowthDuration, GrowthEase);
        slime.SizeScale.SetBaseValue(Mathf.Lerp(slime.SmallSize, 1f, parameter));
    }
}