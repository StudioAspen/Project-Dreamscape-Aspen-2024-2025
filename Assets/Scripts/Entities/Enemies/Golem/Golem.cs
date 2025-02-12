using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    #region States
    public GolemGroundSmashState GolemGroundSmashState { get; private set; }
    public GolemWanderState GolemWanderState { get; private set; }
    public GolemChaseState GolemChaseState { get; private set; }
    public GolemReadyAttackState GolemReadyAttackState {get; private set;}
    public GolemAttackRecoverState GolemAttackRecoverState {get; private set;}
    public GolemStompState GolemStompState {get; private set;}


    private Coroutine camShakeCoroutine;
    
    private protected override void InitializeStates()
    {
        base.InitializeStates();

        GolemGroundSmashState = EntityBaseState.InitializeOrCreate<GolemGroundSmashState>(this);
        GolemWanderState = EntityBaseState.InitializeOrCreate<GolemWanderState>(this);
        GolemAttackRecoverState = EntityBaseState.InitializeOrCreate<GolemAttackRecoverState>(this);
        GolemChaseState = EntityBaseState.InitializeOrCreate<GolemChaseState>(this);
        GolemReadyAttackState = EntityBaseState.InitializeOrCreate<GolemReadyAttackState>(this);
        GolemStompState = EntityBaseState.InitializeOrCreate<GolemStompState>(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        FinishAnimation();
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(GolemWanderState);
        
        FinishAnimation();
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public void FinishAnimation()
    {
        IsAttackAnimationPlaying = false;
        EndHit();
    }

    public void StartHit() { // Called via Animation Event
        GolemGroundSmashState.GroundImpact();
    }

    public void StompImpact() { // called via Animation Event
        GolemStompState.GroundImpactShockwave();
    }

    public void ShakeCam() {
        camShakeCoroutine = StartCoroutine(CamShakeCoroutine(8, .1f / LocalTimeScale));
    }

    public void EndHit()
    {
        GolemGroundSmashState.Weapon.DisableTriggers();
    }

    private IEnumerator CamShakeCoroutine(int numShakes, float shakeDelay) {
        for (int i = 0; i < numShakes; i++) {
            CameraShakeManager.Instance.ShakeCamera(Random.Range(4f,8f), shakeDelay);    
            yield return new WaitForSeconds(shakeDelay);
        }
    }
    
   
    
}
