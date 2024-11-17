using DG.Tweening;
using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Burn")]
public class BurnStatusEffectSO : StatusEffectSO
{
    [Header("Burn: Settings")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float tickDuration = 0.5f;
    private float tickTimer;

    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();

    private protected override void OnApply()
    {
        base.OnApply();

        entity.TweenTintEntity(Color.red);
    }

    public override void Update()
    {
        base.Update();

        tickTimer += Time.deltaTime;
        if(tickTimer >= tickDuration)
        {
            tickTimer = 0;

            entity.TakeDamageWithoutState(damagePerTick, entity.GetColliderCenterPosition(), source);
        }
    }

    private protected override void OnExpire()
    {
        entity.TweenUnTintEntity();

        base.OnExpire();
    }
}
