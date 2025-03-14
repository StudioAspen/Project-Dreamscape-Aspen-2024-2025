using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities 
{

public class Boulder : CastedAbility, IPoolableObject 
{
    [Header("Settings")] [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float aoeRadius = 2f;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float boulderLifetime = 12f;
    
    [SerializeField] private float playerBounceCooldown = 1f;
    [SerializeField] private float wallBounceCooldown = .15f;
    [SerializeField] private float boulderUnleashDelay = 1.5f;

    private Vector3 direction;
    private float bounceTimer = 0f;
    private float wallBounceTimer = 0f;
    private float bounceHeight = 2f, groundOffset = .75f;

    private new Rigidbody rigidbody;
    private Coroutine moveCoroutine;

    private void Awake() 
    {
        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null) Debug.LogError("Boulder requires a Rigidbody component!");
    }

    private protected override void OnSpawn() {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        direction = casterEntity.transform.forward;
        moveCoroutine = StartCoroutine(BoulderMove());
    }

    
    private protected override void OnOnDisable()
    {
    }

    void OnDrawGizmos()
    {
        //Visualize AOE radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rigidbody.position, aoeRadius);
    }
    
    private IEnumerator BoulderMove() 
    {
        yield return new WaitForSeconds(boulderUnleashDelay);

        float currentTimeElapsed = 0f;
        float verticalVelocity = 0f;
        bool isFalling = true;
        bounceTimer = playerBounceCooldown;

        while (currentTimeElapsed < boulderLifetime) 
        {
            // Calculate next position
            Vector3 nextPosition = rigidbody.position + (direction * (forwardSpeed * Time.deltaTime));

            // Apply gravity if falling
            if (isFalling) 
            {
                verticalVelocity -= 9.81f * Time.deltaTime;
            }

            // Ground collision check with offset
            if (Physics.Raycast(nextPosition + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 1.5f,
                    LayerMask.GetMask("Ground"))) 
            {
                if (nextPosition.y + verticalVelocity * Time.deltaTime <= hit.point.y + groundOffset) 
                {
                    // Snap to ground with offset
                    nextPosition.y = hit.point.y + groundOffset;

                    if (isFalling) 
                    {
                        // Apply bounce
                        verticalVelocity = Mathf.Sqrt(2f * 9.81f * bounceHeight);
                        isFalling = false;
                    }
                }
            } else 
            {
                isFalling = true;
            }

            // Apply vertical movement
            nextPosition.y += verticalVelocity * Time.deltaTime;

            // Move boulder with rigidbody moveposition
            rigidbody.MovePosition(nextPosition);

            // Update timers
            currentTimeElapsed += Time.deltaTime;
            bounceTimer = Mathf.Max(0f, bounceTimer - Time.deltaTime);
            wallBounceTimer = Mathf.Max(0f, wallBounceTimer - Time.deltaTime);

            yield return null;
        }

        Explode();
        if (moveCoroutine != null) moveCoroutine = null;
        if (gameObject) DestroyAndRelease();
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

        if (hitEntity.CurrentState == hitEntity.EntityDeathState) return;
        if (hitEntity.Team == casterEntity.Team) 
        {
            if (bounceTimer == 0f) 
            {
                bounceTimer = playerBounceCooldown;
                Vector3 hitNormal = (rigidbody.position - new Vector3(hitEntity.transform.position.x, rigidbody.position.y,
                    hitEntity.transform.position.z)).normalized;
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

        Debug.DrawRay(rigidbody.position, direction * 10f, Color.green, 100f);

        if (Physics.Raycast(rigidbody.position, direction, out var hit, 10f)) 
        {
            direction = Vector3.Reflect(direction, hit.normal);
            wallBounceTimer = wallBounceCooldown;
        } else 
        {
            direction = -direction;
        }
    }

    private void Explode() 
    {
        List<Entity> entitiesHit = Entity.GetEntitiesThroughAOE(rigidbody.position, aoeRadius, false);

        foreach (Entity entity in entitiesHit) 
        {
            if (entity.Team == casterEntity.Team) return;

            casterEntity.DealDamageToOtherEntity(entity, casterEntity.CalculateDamage(damageMultiplier), rigidbody.position);
        }

        // Insert explosion VFX here:
        CustomDebug.InstantiateTemporarySphere(rigidbody.position, aoeRadius, 0.25f, new Color(1f, 0, 0, 0.2f));
    }

    public void SetBounceHeight(float newHeight) {
        bounceHeight = newHeight;
    }

    public void SetGroundOffset(float newOffset) {
        groundOffset = newOffset;
    }
    
    
    
}

}