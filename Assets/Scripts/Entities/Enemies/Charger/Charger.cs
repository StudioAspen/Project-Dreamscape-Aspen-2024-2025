using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Charger : Enemy
{
    [Header("Charger: Armor Settings")]
    [SerializeField] private int staggerDamageThreshold = 40;
    [SerializeField] private float superArmorDamageReduction = 0.5f; // takes (100 - superArmorDamageReduction)% of damage when hit

    [field: Header("Charger: Wander Settings")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(3f, 5f);

    [field: Header("Charger: Player Detected Settings")]
    [field: SerializeField] public float DetectionDistance { get; private set; } = 15f;
    [field: SerializeField] public float DetectionConeHalfAngle { get; private set; } = 30f;
    [field: SerializeField] public float TargetDetectedDuration { get; private set; } = 2f;
    [field: SerializeField] public float NearbyAttackRadiusThreshold { get; private set; } = 6f;

    [field: Header("Charger: Charge Settings")]
    [field: SerializeField] public float ChargeContactPercentDamage { get; private set; } = 200f;
    [field: SerializeField] public float ChargeSpeedModifier { get; private set; } = 5f;
    [field: SerializeField] public float ChargeDuration { get; private set; } = 20f;
    [field: SerializeField] public float ChargeRotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ChargeOnImpactLaunchForce { get; private set; } = 10f;
    [field: SerializeField] public float ChargeStunDuration { get; private set; } = 4f;
    [field: SerializeField] public LayerMask ChargeLayerMask { get; private set; }

    [field: Header("Charger: Wind Down Settings")]
    [field: SerializeField] public float WindDownDuration { get; private set; } = 2f;

    [field: Header("Charger: Jabbing Attack Settings")]
    [field: SerializeField] public int JabCount { get; private set; } = 5;
    [field: SerializeField] public float JabPercentDamage { get; private set; } = 100;
    [field: SerializeField] public float JabStandStillRadius { get; private set; } = 1.5f;
    [field: SerializeField] public float JabRotationSpeed { get; private set; } = 25f;
    [field: SerializeField] public Weapon LeftFistWeapon { get; private set; }
    [field: SerializeField] public Weapon RightFistWeapon { get; private set; }
    [field: SerializeField] public float JabRecoverDuration { get; private set; } = 2f;
    public int RemainingJabs { get; private set; }

    [field: Header("Charger: Dazed Settings")]
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;

    #region States
    public ChargerWanderState ChargerWanderState { get; private set; }
    public ChargerTargetDetectedState ChargerTargetDetectedState { get; private set; }
    public ChargerChargeState ChargerChargeState { get; private set; }
    public ChargerWindDownState ChargerWindDownState { get; private set; }
    public ChargerDazedState ChargerDazedState { get; private set; }
    public ChargerJabbingAttackState ChargerJabbingAttackState { get; private set; }
    public ChargerJabRecoverState ChargerJabRecoverState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        ChargerWanderState = new ChargerWanderState(this);
        ChargerTargetDetectedState = new ChargerTargetDetectedState(this);
        ChargerChargeState = new ChargerChargeState(this);
        ChargerDazedState = new ChargerDazedState(this);
        ChargerWindDownState = new ChargerWindDownState(this);
        EntityStaggeredState = new ChargerStaggeredState(this);
        ChargerJabbingAttackState = new ChargerJabbingAttackState(this);
        ChargerJabRecoverState = new ChargerJabRecoverState(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(ChargerWanderState);

        FinishAnimation();
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

    private protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    private protected override void OnOnDrawGizmos()
    {
        base.OnOnDrawGizmos();

        Gizmos.color = Color.red;
        CustomGizmos.DrawWireCircle(transform.position, targetDetectionRadius);
        CustomGizmos.DrawWireCircle(transform.position, NearbyAttackRadiusThreshold);
        CustomGizmos.DrawWireCone(CustomCollisionTopPoint, transform.forward, DetectionConeHalfAngle, DetectionDistance);
    }

    public override void TryAssignTarget()
    {
        // replace default radius-based target assignment with cone-based target assignment
        TryAssignTargetWithCone(DetectionDistance, DetectionConeHalfAngle);
    }

    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source, bool willTryStagger = true)
    {
        if (CurrentState == EntityDeathState) return;

        int newDamage = dmg;

        if(HasSuperArmorActive())
        {
            if(dmg >= staggerDamageThreshold)
            {
                ForceChangeState(EntityStaggeredState);
            }
            else
            {
                newDamage = Mathf.RoundToInt(superArmorDamageReduction * dmg);
            }
        }
        else
        {
            if(CanBeStaggered())
            {
                ForceChangeState(EntityStaggeredState);
            }
        }

        OnEntityTakeDamage?.Invoke(newDamage, hitPoint, source);

        CurrentHealth -= newDamage;

        AttemptToSpawnHitNumbers(newDamage, hitPoint, Color.red);

        lastHitSource = source;

        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth > 0)
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

        return MaxHealth > 0 && CurrentHealth - newDamage <= 0;
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
    private bool CanBeStaggered()
    {
        return CurrentState == ChargerWanderState
            || CurrentState == ChargerDazedState
            || CurrentState == ChargerWindDownState
            || CurrentState == ChargerJabRecoverState;
    }

    public void ResetJabCount()
    {
        RemainingJabs = JabCount;
    }

    public void FinishAnimation()
    {
        IsAttackAnimationPlaying = false;
        DisableWeaponTriggers();
    }

    public void EnableWeaponTriggers()
    {
        if (RemainingJabs % 2 == 1)
        {
            RightFistWeapon.EnableTriggers();
        }
        else
        {
            LeftFistWeapon.EnableTriggers();
        }

        RemainingJabs--;
    }

    public void DisableWeaponTriggers()
    {
        RightFistWeapon.DisableTriggers();
        LeftFistWeapon.DisableTriggers();
    }
}
