using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    
    [Header("Charger: Armor Settings")]
    [SerializeField] private int dazeDamageThreshold = 40; // Damage needed to take while staggered in order to become dazed

    [SerializeField] private int staggerDamageThreshold = 15; // Minimum damage needed to take in order to become staggered
    
    private Coroutine camShakeCoroutine;
    private int damageTakenWhileStaggered = 0;
    private int damageTakenSinceLastStagger = 0;
    
    
    #region States
    public GolemGroundSmashState GolemGroundSmashState { get; private set; }
    public GolemWanderState GolemWanderState { get; private set; }
    public GolemChaseState GolemChaseState { get; private set; }
    public GolemReadyAttackState GolemReadyAttackState {get; private set;}
    public GolemAttackRecoverState GolemAttackRecoverState {get; private set;}
    public GolemStompState GolemStompState {get; private set;}
    public GolemStaggeredState GolemStaggeredState {get; private set;}
    public GolemDazedState GolemDazedState {get; private set;}
    

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        GolemGroundSmashState = EntityBaseState.InitializeOrCreate<GolemGroundSmashState>(this);
        GolemWanderState = EntityBaseState.InitializeOrCreate<GolemWanderState>(this);
        GolemAttackRecoverState = EntityBaseState.InitializeOrCreate<GolemAttackRecoverState>(this);
        GolemChaseState = EntityBaseState.InitializeOrCreate<GolemChaseState>(this);
        GolemReadyAttackState = EntityBaseState.InitializeOrCreate<GolemReadyAttackState>(this);
        GolemStompState = EntityBaseState.InitializeOrCreate<GolemStompState>(this);
        GolemStaggeredState = EntityBaseState.InitializeOrCreate<GolemStaggeredState>(this);
        GolemDazedState = EntityBaseState.InitializeOrCreate<GolemDazedState>(this);
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
    
    
    /// <summary>
    /// Determines if the Charger can be staggered based on its current state.
    /// </summary>
    /// <returns>True if the Charger can be staggered, false otherwise.</returns>
    private bool CanBeStaggered() {
        return CurrentState == GolemReadyAttackState
               || CurrentState == GolemStaggeredState
               || CurrentState == GolemAttackRecoverState;
    }
   
    
    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source, bool willTryStagger = true)
    {
        if (CurrentState == EntityDeathState) return;

        int newDamage = dmg;
        
        if(CanBeStaggered()) {
            damageTakenSinceLastStagger += newDamage;
            if (damageTakenSinceLastStagger > staggerDamageThreshold) {
                ResetDamageTakenSinceLastStagger();
                ChangeState(GolemStaggeredState, true);    
            }
        }
        
        if (CurrentState == GolemStaggeredState) {
            damageTakenWhileStaggered += newDamage;
            print("Damage taken while staggered now " + damageTakenWhileStaggered);
            if (damageTakenWhileStaggered > dazeDamageThreshold) {
                ResetDamageTakenWhileStaggered();
                ChangeState(GolemDazedState, true);
            }
        }

        OnEntityTakeDamage?.Invoke(newDamage, hitPoint, source);

        CurrentHealth -= newDamage;

        AttemptToSpawnHitNumbers(newDamage, hitPoint, Color.red);

        lastHitSource = source;
        
        
        //after calculating current health, check if the entity has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    public void ResetDamageTakenWhileStaggered() {
        damageTakenWhileStaggered = 0;
    }

    public void ResetDamageTakenSinceLastStagger() {
        damageTakenSinceLastStagger = 0;
    }
    
    
}
