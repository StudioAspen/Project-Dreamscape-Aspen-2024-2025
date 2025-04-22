using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class Fireball : CastedAbility, IPoolableObject
    {
        [Header("Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private float aoeRadius = 5f;
        [SerializeField] private float damageMultiplier = 1f;

        private Coroutine moveCoroutine;

        private protected override void OnSpawn()
        {
            transform.position = casterEntity.GetColliderCenterPosition();

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(FireballMove(casterEntity.transform.forward));
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

        private IEnumerator FireballMove(Vector3 direction)
        {
            float distanceTraveled = 0f;

            while (distanceTraveled < maxDistance)
            {
                Vector3 moveDistanceThisFrame = speed * Time.deltaTime * direction.normalized;

                transform.Translate(moveDistanceThisFrame);

                distanceTraveled += moveDistanceThisFrame.magnitude;

                yield return null;
            }

            Explode();
            moveCoroutine = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForEntityHit(other);
            CheckForWallHit(other);
        }

        private void CheckForEntityHit(Collider other)
        {
            Entity hitEntity = other.GetComponent<Entity>();
            if (hitEntity == null) hitEntity = other.GetComponentInParent<Entity>();
            if (hitEntity == null) return;

            if (hitEntity.CurrentState == hitEntity.EntityDeathState) return; // if theyre already dying
            if (hitEntity.Team == casterEntity.Team) return; // if theyre on the same team

            Explode();
        }

        private void CheckForWallHit(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;

            Explode();
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

            DestroyAndRelease();
        }
    }
}