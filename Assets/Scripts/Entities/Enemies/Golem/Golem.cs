using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    [Header("Golem: Armor Settings")]
    [SerializeField] private int dazeDamageThreshold = 40; // Damage needed to take while staggered in order to become dazed
    [SerializeField] private int staggerDamageThreshold = 15; // Minimum damage needed to take in order to become staggered
    
    private Coroutine camShakeCoroutine;
    private int damageTakenWhileStaggered = 0;
    private int damageTakenSinceLastStagger = 0;

    #region States
    [field: Header("Golem: States")]
    [field: SerializeField] public GolemGroundSmashState GolemGroundSmashState { get; private set; }
    [field: SerializeField] public GolemWanderState GolemWanderState { get; private set; }
    [field: SerializeField] public GolemChaseState GolemChaseState { get; private set; }
    [field: SerializeField] public GolemReadyAttackState GolemReadyAttackState {get; private set;}
    [field: SerializeField] public GolemAttackRecoverState GolemAttackRecoverState {get; private set;}
    [field: SerializeField] public GolemStompState GolemStompState {get; private set;}
    [field: SerializeField] public GolemStaggeredState GolemStaggeredState {get; private set;}
    [field: SerializeField] public GolemDazedState GolemDazedState {get; private set;}
    
    private protected override void InitializeStates()
    {
        base.InitializeStates();

        GolemGroundSmashState.Init(this);
        GolemWanderState.Init(this);
        GolemAttackRecoverState.Init(this);
        GolemChaseState.Init(this);
        GolemReadyAttackState.Init(this);
        GolemStompState.Init(this);
        GolemStaggeredState.Init(this);
        GolemDazedState.Init(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(GolemWanderState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    // Called via GroundSmash Animation Event
    public void GroundSmashImpact() 
    { 
        GolemGroundSmashState.GroundImpact();
    }

    // Called via Stomp Animation Event
    public void StompImpact() 
    { 
        GolemStompState.GroundImpactShockwave();
    }

    /// <summary>
    /// Determines if the Charger can be staggered based on its current state.
    /// </summary>
    /// <returns>True if the Charger can be staggered, false otherwise.</returns>
    public override bool CanBeStaggered()
    {
        return CurrentState == GolemReadyAttackState
               || CurrentState == GolemStaggeredState
               || CurrentState == GolemAttackRecoverState;
    }
   
    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source, bool willTryStagger = true, bool willIgnoreDefense = false)
    {
        if (CurrentState == EntityDeathState) return;

        if (willIgnoreDefense) dmg = Mathf.Clamp(dmg - Defense.GetIntValue(), 0, int.MaxValue);

        if (CanBeStaggered())
        {
            damageTakenSinceLastStagger += dmg;
            if (damageTakenSinceLastStagger > staggerDamageThreshold)
            {
                ResetDamageTakenSinceLastStagger();
                ChangeState(GolemStaggeredState, true);    
            }
        }
        
        if (CurrentState == GolemStaggeredState)
        {
            damageTakenWhileStaggered += dmg;
            //print("Damage taken while staggered now " + damageTakenWhileStaggered);
            if (damageTakenWhileStaggered > dazeDamageThreshold)
            {
                ResetDamageTakenWhileStaggered();
                ChangeState(GolemDazedState, true);
            }
        }

        if(!IsInvicible) CurrentHealth -= dmg;
        OnEntityTakeDamage?.Invoke(dmg, hitPoint, source);
        AttemptToSpawnHitNumbers(dmg, hitPoint, Color.red);
        lastHitSource = source;
        
        //after calculating current health, check if the entity has taken enough damage to die
        if (CurrentHealth <= 0 && !IsInvicible)
        {
            OnDeath();
        }
    }

    public void ResetDamageTakenWhileStaggered()
    {
        damageTakenWhileStaggered = 0;
    }

    public void ResetDamageTakenSinceLastStagger()
    {
        damageTakenSinceLastStagger = 0;
    }
}
