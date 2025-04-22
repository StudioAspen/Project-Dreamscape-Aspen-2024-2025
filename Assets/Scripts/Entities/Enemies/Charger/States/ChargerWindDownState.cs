using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChargerWindDownState : ChargerBaseState
{
    [field: SerializeField] public float WindDownDuration { get; private set; } = 2f;

    private float timer;
    private float halfWindDownDuration;

    public override void OnEnter()
    {
        charger.PlayDefaultAnimation();

        timer = 0f;
        halfWindDownDuration = WindDownDuration / 2f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        charger.ApplyGravity();

        timer += charger.LocalDeltaTime;

        if (timer > WindDownDuration)
        {
            charger.ChangeState(charger.ChargerWanderState);
            return;
        }

        if(timer < halfWindDownDuration)
        {
            float easedSpeedModifier = DOVirtual.EasedValue(charger.ChargerChargeState.ChargeSpeedModifier, 0, timer / halfWindDownDuration, Ease.OutQuad);
            charger.SetSpeedModifier(easedSpeedModifier);

            CheckCollisions();
        }

        charger.UpdateHorizontalVelocity(charger.transform.forward);
        charger.ApplyHorizontalVelocity();
    }

    private void CheckCollisions()
    {
        List<Collider> orderedHits = charger.GetCustomCollisionHits(charger.ChargerChargeState.ChargeLayerMask);

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
