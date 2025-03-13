using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class Boulder : CastedAbility, IPoolableObject
    {
        [Header("Settings")]
        [SerializeField] private float forwardSpeed = 5f;
        [SerializeField] private float aoeRadius = 2f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float boulderLifetime = 12f;
        [SerializeField] private float bounceHeight = 2f;
        [SerializeField] private float spawnForwardOffset = 2f;
        [SerializeField] private float groundOffset = .75f;

        [SerializeField] private float playerBounceCooldown = 1f;
        [SerializeField] private float wallBounceCooldown = .15f;
        [SerializeField] private float boulderUnleashDelay = 1.5f;
        
        private Vector3 direction;
        private float bounceTimer = 0f;
        private float wallBounceTimer = 0f;
        
        private Coroutine moveCoroutine;

        private protected override void OnSpawn()
        {
            transform.position = casterEntity.GetColliderCenterPosition();

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            direction = casterEntity.transform.forward;
            transform.position += (direction * spawnForwardOffset) + (Vector3.up * (groundOffset + bounceHeight));
            moveCoroutine = StartCoroutine(BoulderMove());
        }

        private protected override void OnOnDisable()
        {

        }

        void OnDrawGizmos()
        {
            //Visualize AOE radius in the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aoeRadius);
        }
        
        private IEnumerator BoulderMove() {
            yield return new WaitForSeconds(boulderUnleashDelay);
            
            float currentTimeElapsed = 0f;
            float verticalVelocity = 0f;
            bool isFalling = true;
            bounceTimer = playerBounceCooldown;

            while (currentTimeElapsed < boulderLifetime) {
                // Move forward
                Vector3 nextPosition = transform.position + (direction * (forwardSpeed * Time.deltaTime));

                // Apply gravity if falling
                if (isFalling) {
                    verticalVelocity -= 9.81f * Time.deltaTime;
                }

                // Ground collision check with offset
                if (Physics.Raycast(nextPosition + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 1.5f, LayerMask.GetMask("Ground"))) {
                    if (nextPosition.y + verticalVelocity * Time.deltaTime <= hit.point.y + groundOffset) {
                        // Snap to ground with offset
                        nextPosition.y = hit.point.y + groundOffset;

                        if (isFalling) {
                            // Apply bounce
                            verticalVelocity = Mathf.Sqrt(2f * 9.81f * bounceHeight);
                            isFalling = false;
                        }
                    }
                } else {
                    isFalling = true;
                }

                // Apply vertical movement
                nextPosition.y += verticalVelocity * Time.deltaTime;

                // Update position
                transform.position = nextPosition;
                
                // Update time elapsed
                currentTimeElapsed += Time.deltaTime;
                // print(currentTimeElapsed + " / " + boulderLifetime);
                
                // Update bounce timer
                bounceTimer = (bounceTimer > 0f) ? bounceTimer - Time.deltaTime : 0f;
                wallBounceTimer = (wallBounceTimer > 0f) ? wallBounceTimer - Time.deltaTime : 0f;

                yield return null;
            }
            
            Explode();
            if (moveCoroutine != null) moveCoroutine = null;
            if (gameObject) {
                DestroyAndRelease();
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            CheckForEntityHit(other);
            CheckForBarrierHit(other);
        }

        private void CheckForEntityHit(Collider other)
        {
            Entity hitEntity = other.GetComponent<Entity>();
            if (hitEntity == null) hitEntity = other.GetComponentInParent<Entity>();
            if (hitEntity == null) return;
            
            if (hitEntity.CurrentState == hitEntity.EntityDeathState) return; // if theyre already dying
            if (hitEntity.Team == casterEntity.Team) { // if theyre on the same team
                
                if (bounceTimer == 0f) {
                    bounceTimer = playerBounceCooldown;
                    Vector3 hitNormal = (transform.position - new Vector3(hitEntity.transform.position.x, transform.position.y, hitEntity.transform.position.z) ).normalized;
                    direction = Vector3.Reflect(direction, hitNormal);
                } 
                return;
            }
            Explode();
        }
        
        
        
        private void CheckForBarrierHit(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
            if (!other.CompareTag("Border")) return;
            if (wallBounceTimer != 0f) return;
            Debug.DrawRay(transform.position, direction * 10f, Color.green, 100f);

            if (Physics.Raycast(transform.position, direction, out var hit, 10f))
            { 
                direction = Vector3.Reflect(direction, hit.normal);
                wallBounceTimer = wallBounceCooldown;
            } else { // in case the raycast fails to detect anything
                direction = -direction;
            }
        }



        private void Explode()
        {
            List<Entity> entitiesHit = Entity.GetEntitiesThroughAOE(transform.position, aoeRadius, false);

            foreach (Entity entity in entitiesHit)
            {
                if (entity.Team == casterEntity.Team) return;

                casterEntity.DealDamageToOtherEntity(entity, casterEntity.CalculateDamage(damageMultiplier), transform.position);
            }

            //insert explosion vfx here:
            CustomDebug.InstantiateTemporarySphere(transform.position, aoeRadius, 0.25f, new Color(1f, 0, 0, 0.2f));
        }

       
        
    }
}