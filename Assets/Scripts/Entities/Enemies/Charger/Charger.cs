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

        //after calculating current health, check if the player has taken enough damage to die
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

                    //do dmg.

                    ChangeState(prepareChargeAttackState); ///done for testing (should be winddown state)
                }
            }
            //colliding with wall.
            else if (hitCollider.CompareTag("Wall"))
            {
                Debug.Log("Charger hit a wall!");
                Vector3 knockbackDirection = (transform.position - hitCollider.transform.position).normalized;
                ApplyKnockback(this, knockbackDirection, KnockbackForce);

                //take small self dmg.

                ChangeState(prepareChargeAttackState); ///done for testing (should be winddown state)
            }
        }
    }

    private void ApplyKnockback(Entity target, Vector3 direction, float force)
    {
        //player is kinematic.
        if (target is Player player)
        {
            player.transform.position += direction * force * Time.fixedDeltaTime;
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
}
