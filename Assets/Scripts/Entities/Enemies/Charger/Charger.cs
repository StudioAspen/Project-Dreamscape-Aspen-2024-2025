using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    [field: Header("Charger: Cone Detection Settings")]
    [field: SerializeField] public float DetectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 30f;

    [Header("Charger: Armor Settings")]
    [SerializeField] private int staggerDamageThreshold = 40;
    [SerializeField] private float superArmorDamageReduction = 0.5f; // takes (100 - superArmorDamageReduction)% of damage when hit

    #region States
    [field: Header("Charger: States")]
    [field: SerializeField] public ChargerWanderState ChargerWanderState { get; private set; }
    [field: SerializeField] public ChargerTargetDetectedState ChargerTargetDetectedState { get; private set; }
    [field: SerializeField] public ChargerChargeState ChargerChargeState { get; private set; }
    [field: SerializeField] public ChargerWindDownState ChargerWindDownState { get; private set; }
    [field: SerializeField] public ChargerDazedState ChargerDazedState { get; private set; }
    [field: SerializeField] public ChargerJabbingAttackState ChargerJabbingAttackState { get; private set; }
    [field: SerializeField] public ChargerJabRecoverState ChargerJabRecoverState { get; private set; }
    [field: SerializeField] public ChargerStaggeredState ChargerStaggeredState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        ChargerWanderState.Init(this);
        ChargerTargetDetectedState.Init(this);
        ChargerChargeState.Init(this);
        ChargerDazedState.Init(this);
        ChargerWindDownState.Init(this);
        ChargerJabbingAttackState.Init(this);
        ChargerJabRecoverState.Init(this);
        ChargerStaggeredState.Init(this);
        EntityStaggeredState = ChargerStaggeredState;
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

        SetDefaultState(ChargerWanderState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    private protected override void OnTick()
    {
        // dont inherit base to leave target assignment to certain states
    }

    private protected override void OnOnDrawGizmos()
    {
        base.OnOnDrawGizmos();

#if UNITY_EDITOR
        Gizmos.color = Color.red;
        CustomDebug.DrawWireCircle(transform.position, targetDetectionRadius);
        CustomDebug.DrawWireCircle(transform.position, ChargerTargetDetectedState.NearbyAttackRadiusThreshold);
        CustomDebug.DrawWireCone(CustomCollisionTopPoint, transform.forward, DetectionConeHalfAngle, DetectionDistance);
#endif
    }

    public override void TryAssignTarget()
    {
        // replace default radius-based target assignment with cone-based target assignment
        TryAssignTargetWithCone(DetectionDistance, DetectionConeHalfAngle);
    }

    public override void TakeDamage(int damage, Vector3 hitPoint, GameObject source, bool willTryStagger = true, bool willIgnoreDefense = false)
    {
        if (CurrentState == EntityDeathState) return;

        int newDamage = damage;

        if(HasSuperArmorActive())
        {
            if(damage >= staggerDamageThreshold)
            {
                if(willTryStagger) ChangeState(EntityStaggeredState, true);
            }
            else
            {
                newDamage = Mathf.RoundToInt(superArmorDamageReduction * damage);
            }
        }

        if (willIgnoreDefense) newDamage = Mathf.Clamp(newDamage - Defense.GetIntValue(), 0, int.MaxValue);


        if (willTryStagger) TryChangeStaggeredState();

        if(!IsInvicible) CurrentHealth -= newDamage;

        OnEntityTakeDamage?.Invoke(newDamage, hitPoint, source);

        AttemptToSpawnHitNumbers(newDamage, hitPoint, Color.red);

        lastHitSource = source;

        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && !IsInvicible)
        {
            OnDeath();
        }
    }

    public override bool WillDieFromDamage(int damage)
    {
        int newDamage = damage;

        if (HasSuperArmorActive())
        {
            if (damage < staggerDamageThreshold) newDamage = Mathf.RoundToInt(superArmorDamageReduction * damage);
        }

        return MaxHealth.GetIntValue() > 0 && CurrentHealth - newDamage <= 0;
    }

    /// <summary>
    /// Determines if the Charger has super armor active based on its current state.
    /// </summary>
    /// <returns>True if the Charger has super armor active, false otherwise.</returns>
    private bool HasSuperArmorActive()
    {
        return CurrentState == ChargerChargeState
            || CurrentState == ChargerJabbingAttackState
            || CurrentState == ChargerTargetDetectedState;
    }

    /// <summary>
    /// Determines if the Charger can be staggered based on its current state.
    /// </summary>
    /// <returns>True if the Charger can be staggered, false otherwise.</returns>
    public override bool CanBeStaggered()
    {
        if (HasSuperArmorActive()) return false;

        return CurrentState == ChargerWanderState
            || CurrentState == ChargerDazedState
            || CurrentState == ChargerWindDownState
            || CurrentState == ChargerJabRecoverState;
    }

    public void StartHit()
    {
        if (ChargerJabbingAttackState.RemainingJabs % 2 == 1)
        {
            ChargerJabbingAttackState.RightFistWeapon.EnableTriggers();
        }
        else
        {
            ChargerJabbingAttackState.LeftFistWeapon.EnableTriggers();
        }

        ChargerJabbingAttackState.DecrementJabCount();
    }

    public void EndHit()
    {
        ChargerJabbingAttackState.RightFistWeapon.DisableTriggers();
        ChargerJabbingAttackState.LeftFistWeapon.DisableTriggers();
    }

    public override void PlayFootstepLeft()
    {
        AkSoundEngine.PostEvent("Play_ChargerFootstepLeft", gameObject);
    }

    public override void PlayFootstepRight()
    {
        AkSoundEngine.PostEvent("Play_ChargerFootstepRight", gameObject);
    }
}
