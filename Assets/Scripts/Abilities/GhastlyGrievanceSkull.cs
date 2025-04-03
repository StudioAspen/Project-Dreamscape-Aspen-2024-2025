using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class GhastlyGrievanceSkull : CastedAbility, IPoolableObject
    {
        [Header("Settings")]
        [SerializeField] private ExtendedDebuffStatusEffectSO ghastlyGrievanceStatusEffect;
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxDistance = 10f;

        private Coroutine moveCoroutine;

        private protected override void OnSpawn()
        {
            transform.position = casterEntity.GetColliderCenterPosition();

            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveCoroutine(casterEntity.transform.forward));
        }

        private protected override void OnOnDisable()
        {

        }

        private IEnumerator MoveCoroutine(Vector3 direction)
        {
            float distanceTraveled = 0f;

            while (distanceTraveled < maxDistance)
            {
                Vector3 moveDistanceThisFrame = speed * Time.deltaTime * direction.normalized;

                transform.Translate(moveDistanceThisFrame);

                distanceTraveled += moveDistanceThisFrame.magnitude;

                yield return null;
            }

            moveCoroutine = null;

            DestroyAndRelease();
        }

        private void OnTriggerEnter(Collider other)
        {
            Entity hitEntity = other.GetComponent<Entity>();
            if (hitEntity == null) hitEntity = other.GetComponentInParent<Entity>();
            if (hitEntity == null) return;

            if (hitEntity.CurrentState == hitEntity.EntityDeathState) return; // if theyre already dying
            if (hitEntity.Team == casterEntity.Team) return; // if theyre on the same team

            EntityStatusEffector.TryApplyStatusEffect(hitEntity.gameObject, ghastlyGrievanceStatusEffect, casterEntity.gameObject);
            Debug.Log($"Ghastly grievanced {hitEntity.gameObject.name}");
        }
    }
}