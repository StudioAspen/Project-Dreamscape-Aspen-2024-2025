using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class Boulder : CastedAbility, IPoolableObject
    {
        private Rigidbody rigidBody;

        [Header("Settings")][SerializeField] private float speed = 5f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float boulderLifetime = 12f;
        [SerializeField] private float groundBounceSlopeLimit = 45f;

        private float lifetimeTimer;
        private float bounceHeight;
        private HashSet<Entity> hitEnemies = new HashSet<Entity>();

        private bool willCheckForAllies = false; // Used so that the boulder can check for ally hits only once after hitting a wall

        public void SetBounceHeight(float bounceHeight)
        {
            this.bounceHeight = bounceHeight;
        }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            Debug.Assert(rigidBody != null, "Boulder requires a Rigidbody component!");
        }

        private protected override void OnSpawn()
        {
            lifetimeTimer = 0f;

            rigidBody.velocity = speed * casterEntity.transform.forward;
            
            hitEnemies.Clear();

            willCheckForAllies = false;
        }

        private protected override void OnOnDisable()
        {
            
        }

        private void Update()
        {
            lifetimeTimer += Time.deltaTime;
            if(lifetimeTimer > boulderLifetime)
            {
                DestroyAndRelease();
                return;
            }
        }

        private void FixedUpdate()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForAllyHit(other);
            CheckForEnemyHit(other);
            CheckForBarrierHit(other);
        }

        private void CheckForAllyHit(Collider other)
        {
            if (!willCheckForAllies) return;

            if(casterEntity.DidHitFriendlyEntity(other, out Entity allyEntity))
            {
                Vector3 hitNormal = transform.position - other.ClosestPoint(transform.position);
                Reflect(hitNormal);
            }
        }

        private void CheckForEnemyHit(Collider other)
        {
            if(casterEntity.DidHitEnemyEntity(other, out Entity enemyEntity))
            {
                if (hitEnemies.Contains(enemyEntity)) return; // Already hit this enemy (to prevent multiple hits on same enemy)
                hitEnemies.Add(enemyEntity);

                int damage = casterEntity.CalculateDamage(damageMultiplier);
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                casterEntity.DealDamageToOtherEntity(enemyEntity, damage, hitPoint, true);
            }
        }

        private void CheckForBarrierHit(Collider other)
        {
            if (!casterEntity.DidHitWall(other)) return;

            Vector3 hitNormal = transform.position - other.ClosestPoint(transform.position);
            Reflect(hitNormal, Vector3.Angle(hitNormal, Vector3.up) < groundBounceSlopeLimit);
        }

        private void Reflect(Vector3 hitNormal, bool willForceBounceHeight = false)
        {
            willCheckForAllies = true;

            Vector3 reflectedVelocity = Vector3.Reflect(rigidBody.velocity, hitNormal.normalized);
            if (willForceBounceHeight)
            {
                reflectedVelocity.y = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * bounceHeight); // Ensures bounce stays strong
            }

            Vector3 groundedVelocity = new Vector3(reflectedVelocity.x, 0f, reflectedVelocity.z).normalized * speed;

            rigidBody.velocity = groundedVelocity + reflectedVelocity.y * Vector3.up;

            hitEnemies.Clear();
        }
    }
}