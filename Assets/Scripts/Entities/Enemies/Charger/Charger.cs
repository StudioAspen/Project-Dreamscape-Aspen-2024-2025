using System.Collections.Generic;
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
    [field: SerializeField] public Vector2Int ChargeContactDamageRange { get; private set; } = new Vector2Int(20, 30);
    [field: SerializeField] public float ChargeSpeedModifier { get; private set; } = 5f;
    [field: SerializeField] public float ChargeDuration { get; private set; } = 20f;
    [field: SerializeField] public float ChargeRotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float ChargeCollisionRadius { get; private set; } = 2f;
    [field: SerializeField] public float ChargeCollisionOffsetFromGroundDistance { get; private set; } = 0.5f;
    [field: SerializeField] public LayerMask ChargeLayerMask { get; private set; }
    [field: SerializeField] public float ChargeFlingForce { get; private set; } = 10f;
    [field: SerializeField] public float ChargeStunDuration { get; private set; } = 4f;
    public Vector3 ChargeCollisionBottomPoint => GetColliderCenterPosition() - (capsuleCollider.height / 2 - ChargeCollisionRadius - ChargeCollisionOffsetFromGroundDistance) * Vector3.up;
    public Vector3 ChargeCollisionTopPoint => GetColliderCenterPosition() + (capsuleCollider.height / 2 - ChargeCollisionRadius) * Vector3.up;
    private float originalRotationSpeed;

    [field: Header("Charger: Wind Down Settings")]
    [field: SerializeField] public float WindDownDuration { get; private set; } = 2f;

    [field: Header("Charger: Jabbing Attack Settings")]
    [field: SerializeField] public int JabCount { get; private set; } = 5;
    [field: SerializeField] public Vector2Int JabDamageRange { get; private set; } = new Vector2Int(10, 15);
    [field: SerializeField] public float JabStandStillRadius { get; private set; } = 1.5f;
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

    protected override void InitializeStates()
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

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        SetStartState(ChargerWanderState);
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(ChargerWanderState);

        FinishAnimation();

        originalRotationSpeed = rotationSpeed; // cache original rotation speed;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    protected override void OnTick()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        CustomGizmos.DrawWireCircle(transform.position, targetDetectionRadius);
        CustomGizmos.DrawWireCircle(transform.position, NearbyAttackRadiusThreshold);
        CustomGizmos.DrawWireCone(ChargeCollisionTopPoint, transform.forward, DetectionConeHalfAngle, DetectionDistance);

        Gizmos.color = Color.white;
        CustomGizmos.DrawWireCapsule(ChargeCollisionBottomPoint, ChargeCollisionTopPoint, ChargeCollisionRadius);
    }

    protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    public void SetRotationSpeed(float newRotationSpeed)
    {
        rotationSpeed = newRotationSpeed;
    }

    public void ResetRotationSpeed()
    {
        rotationSpeed = originalRotationSpeed;
    }

    public override void TryAssignTarget()
    {
        List<Entity> smallRadiusTargets = GetNearbyTargets();
        List<Entity> largeRadiusTargets = GetNearbyEntities(DetectionDistance);
        List<Entity> filteredTargetsByCone = FilterTargetsInConeShape(largeRadiusTargets, ChargeCollisionTopPoint, DetectionConeHalfAngle);

        if (largeRadiusTargets.Count == 0)
        {
            Target = null;
            return;
        }

        if(filteredTargetsByCone.Count > 0)
        {
            Target = filteredTargetsByCone[0];
            return;
        }

        if (smallRadiusTargets.Count > 0)
        {
            Target = smallRadiusTargets[0];
            return;
        }

        Target = null;
        return;
    }

    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source)
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

        CurrentHealth -= newDamage;

        AttemptToSpawnHitNumbers(newDamage, hitPoint, Color.red);

        lastHitSource = source;

        OnEntityTakeDamage?.Invoke(hitPoint, source);

        //after calculating current health, check if the player has taken enough damage to die
        if (CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    private bool HasSuperArmorActive()
    {
        return CurrentState == ChargerChargeState
            || CurrentState == ChargerJabbingAttackState
            || CurrentState == ChargerTargetDetectedState;
    }

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
