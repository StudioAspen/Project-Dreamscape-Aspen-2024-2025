using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Charger : Enemy
{
    [field: Header("Charger: Attack Settings")]

    [field: SerializeField] public float ChargeSpeed { get; private set; } = 9f;
    [field: SerializeField] public float SpeedHoldTime { get; private set; } = 1f;


    [field: SerializeField] public float ChargeDuration { get; private set; } = 1f;
    [field: SerializeField] public float ChargeTurnRate { get; private set; } = 45f;

    [field: SerializeField] public float SlowDownDuration { get; private set; } = 1f;
    [field: SerializeField] public float SlowDownTurnRate { get; private set; } = 25f;

    [field: SerializeField] public LayerMask hitLayer { get; private set; }
    [field: SerializeField] public GameObject HitBoxLocation { get; private set; }
    [field: SerializeField] public float HitboxRadius { get; private set; } = 1.4f;
    [field: SerializeField] public float KnockbackForce { get; private set; } = 35f;

    [field: SerializeField] public int ChargeDamage { get; private set; } = 10;
    [field: SerializeField] public int SelfDamage { get; private set; } = 2;

    [field: SerializeField] public int DamageThreshold { get; private set; } = 40;
    private int damageDuringCharge = 0;



    #region States
    public prepareChargeAttackState prepareChargeAttackState { get; private set; }
    public ChargeAttackState ChargeAttackState { get; private set; }
    public SlowdownChargeAttackState SlowdownChargeAttackState { get; private set; }


    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(prepareChargeAttackState);
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(EnemyIdleState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        prepareChargeAttackState = new prepareChargeAttackState(this);
        ChargeAttackState = new ChargeAttackState(this);
        SlowdownChargeAttackState = new SlowdownChargeAttackState(this);
    }

    public override void TakeDamage(int dmg, Vector3 hitPoint, GameObject source)
    {
        if (CurrentState == EntityDeathState) return;

        AttemptToSpawnHitNumbers(dmg, hitPoint);
        CurrentHealth -= dmg;
        lastHitSource = source;
        OnEntityTakeDamage?.Invoke(hitPoint, source);

        //for superarmor checks.
        if (CurrentState == ChargeAttackState || CurrentState == SlowdownChargeAttackState)
        {
            damageDuringCharge += dmg;
            SuperArmorThreshold();
        }

        if (CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    //visual check for hitbox.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(HitBoxLocation.transform.position, HitboxRadius);
    }


    public void CheckForHits()
    {
        //sphere on charger position.
        Collider[] hitColliders = Physics.OverlapSphere(HitBoxLocation.transform.position, HitboxRadius, hitLayer);

        foreach (var hitCollider in hitColliders)
        {
            Entity hitEntity = hitCollider.GetComponent<Entity>();
            Vector3 hitPoint = hitCollider.ClosestPointOnBounds(HitBoxLocation.transform.position);

            if (hitEntity != null)
            {
                //colliding with another enemy.
                if (hitEntity.Team == Team)
                {
                    Vector3 knockbackDirection = (hitEntity.transform.position - transform.position).normalized;
                    ApplyKnockback(hitEntity, knockbackDirection, KnockbackForce);
                }
                //colliding with target/player.
                else
                {
                    Debug.Log("Player has been hit by charger!");
                    Vector3 knockbackDirection = (hitEntity.transform.position - transform.position).normalized;
                    ApplyKnockback(hitEntity, knockbackDirection, KnockbackForce);

                    hitEntity.TakeDamage(ChargeDamage, hitPoint, gameObject);

                    ChangeState(prepareChargeAttackState); ///done for testing (should be winddown state)
                }
            }
            //colliding with wall.
            else if (hitCollider.CompareTag("Wall"))
            {
                Debug.Log("Charger hit a wall!");
                Vector3 knockbackDirection = (transform.position - hitCollider.transform.position).normalized;
                ApplyKnockback(this, knockbackDirection, KnockbackForce);

                TakeDamage(SelfDamage, hitPoint, gameObject);

                ChangeState(prepareChargeAttackState); ///done for testing (should be winddown state)
            }
        }
    }

    private void ApplyKnockback(Entity target, Vector3 direction, float force)
    {
        //player is kinematic.
        if (target is Player player)
        {
            float knockbackDistance = force * Time.fixedDeltaTime;
            float collisionCheckDistance = knockbackDistance + 0.1f;

            //raycast check so in the case the player collides with a wall, they are not pushed fully through.
            RaycastHit hit;
            bool isBlocked = Physics.Raycast(player.transform.position, direction, out hit, collisionCheckDistance);

            //when the collisionCheck is hit.
            if (isBlocked)
            {
                player.transform.position += direction * (hit.distance - 0.1f);
            }
            //if not apply full knockback as normal.
            else
            {
                player.transform.position += direction * knockbackDistance;
            }        
        }
        //enemies are not kinematic.
        else
        {
            Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();

            if (targetRigidbody != null)
            {
                targetRigidbody.AddForce(direction * force, ForceMode.Impulse);
            }
        }
    }

    private void SuperArmorThreshold()
    {
        if (damageDuringCharge >= DamageThreshold)
        {
            Debug.Log("Enemy has taken enough damage: Enter Damaged State");
            ChangeState(prepareChargeAttackState); //done for testing (should be damaged state).
        }
    }

    //called at the exit of slowdown so that it is not carried over during the next charge attack if applicable.
    public void ResetDamageTracking()
    {
        damageDuringCharge = 0;
    }
}
