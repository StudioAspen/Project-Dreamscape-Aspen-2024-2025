using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class WeaponTipAoeExplosion : CastedAbility
    {
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private bool willTryStagger = true;

        [Header("Launch Config")]
        [SerializeField] private bool willLaunch;
        [SerializeField] private float launchForce;
        [SerializeField] private float launchStunDuration;

        private protected override void OnSpawn()
        {
            Weapon weapon = casterEntity.GetComponentInChildren<Weapon>();
            if (weapon == null)
            {
                DestroyAndRelease();
                return;
            }

            if(weapon.TipTransform == null)
            {
                DestroyAndRelease();
                return;
            }

            if (willLaunch)
            {
                Entity.DamageEnemyEntitiesWithAOELaunch(casterEntity, weapon.TipTransform.position, explosionRadius, damageMultiplier, launchForce, launchStunDuration);
            }
            else
            {
                Entity.DamageEnemyEntitiesWithAOE(casterEntity, weapon.TipTransform.position, explosionRadius, damageMultiplier, willTryStagger);
            }

            CameraShakeManager.Instance.ShakeCamera(10f, 1f, 0.75f);
            CustomDebug.InstantiateTemporarySphere(weapon.TipTransform.position, explosionRadius, 1, new Color(1f, 0, 0, 0.25f));
            DestroyAndRelease();
        }

        private protected override void OnOnDisable()
        {
           
        }
    }
}
