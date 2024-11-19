using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShielderPlayerDetectedState : EnemyBaseState
{
    private Shielder shielder;

    private Entity rememberedTarget;

    private float timer;

    public ShielderPlayerDetectedState(Shielder enemy) : base(enemy)
    {
        shielder = enemy;
    }

    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        shielder.DefaultTransitionToAnimation("TargetDetected");

        shielder.SetSpeedModifier(0f);

        timer = 0f;
    }

    public override void OnExit() { }

    public override void Update()
    {
        timer += Time.deltaTime;

        shielder.LookAt(rememberedTarget.transform.position);
        
        if(timer > shielder.PlayerDetectedDuration)
        {
            // Enter a defensive state
            Debug.Log("Switch to defend state");
            //shielder.ShielderDefendState.AssignCurrentRememberedTarget(rememberedTarget);
            //shielder.ChangeState(shielder.ShielderDefendState);
            return;
        }
    }

    public override void FixedUpdate() { }
}
