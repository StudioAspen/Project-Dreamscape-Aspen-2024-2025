using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargerWindDownState : EnemyBaseState
{
    private Charger charger;

    private float timer;
    private float halfWindDownDuration;

    public ChargerWindDownState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        charger.TransitionToAnimation("FlatMovement");

        timer = 0f;
        halfWindDownDuration = charger.WindDownDuration / 2f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        charger.ApplyGravity();

        timer += charger.LocalDeltaTime;

        if (timer > charger.WindDownDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        if(timer < halfWindDownDuration)
        {
            float easedSpeedModifier = DOVirtual.EasedValue(charger.ChargeSpeedModifier, 0, timer / halfWindDownDuration, Ease.OutQuad);
            charger.SetSpeedModifier(easedSpeedModifier);

            CheckCollisions();
        }

        charger.UpdateHorizontalVelocity(charger.transform.forward);
        charger.ApplyHorizontalVelocity();
    }

    public override void FixedUpdate()
    {

    }

    private void CheckCollisions()
    {
        List<Collider> orderedHits = charger.GetCustomCollisionHits(charger.ChargeLayerMask);

        foreach (Collider hit in orderedHits)
        {
            if (charger.DidHitWall(hit))
            {
                charger.ChangeState(charger.ChargerDazedState);
                return;
            }
        }
    }
}
