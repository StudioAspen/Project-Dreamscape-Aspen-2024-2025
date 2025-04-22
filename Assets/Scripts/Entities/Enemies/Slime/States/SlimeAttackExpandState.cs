using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlimeAttackExpandState : SlimeBaseState
{
    [field: SerializeField] public LayerMask SlimeAttackLayerMask { get; private set; }
    [field: SerializeField] public float AttackContactDamageMultiplier { get; private set; } = 1.5f;
    [field: SerializeField] public float AttackExpandSize { get; private set; } = 2f;
    [field: SerializeField] public float AttackExpandDuration { get; private set; } = 0.3f;
    [field: SerializeField] public Ease ExpandEase { get; private set; } = Ease.OutCubic;

    private float timer;
    private Entity rememberedTarget;
    private List<Entity> entitiesHitByCurrentAttack = new List<Entity>();
  
    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        slime.SetSpeedModifier(0f);

        entitiesHitByCurrentAttack.Clear();

        timer = 0f;
    }

    public override void OnExit()
    {
        rememberedTarget = null;
        slime.SizeScale.SetBaseValue(AttackExpandSize);
    }

    public override void OnUpdate()
    {
        slime.ApplyGravity();

        timer += slime.LocalDeltaTime;
        if(timer > AttackExpandDuration)
        {
            slime.ChangeState(slime.SlimeAttackShrinkState);
            return;
        }

        slime.CheckCollisions(AttackContactDamageMultiplier, ref entitiesHitByCurrentAttack);

        float parameter = DOVirtual.EasedValue(0f, 1f, timer / AttackExpandDuration, ExpandEase);
        float currentSize = slime.IsSmall ? slime.SmallSize : 1f;
        slime.SizeScale.SetBaseValue(Mathf.Lerp(currentSize, currentSize * AttackExpandSize, parameter));
    }
}
