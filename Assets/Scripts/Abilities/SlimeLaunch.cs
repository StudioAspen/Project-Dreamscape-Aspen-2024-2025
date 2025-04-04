using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class SlimeLaunch : CastedAbility, IPoolableObject
    {
        [Header("Settings")]
        [SerializeField] private float slimeIndex = 1; //Index used to determine if slime can split
        [SerializeField] private int numberOfSlimes = 8;
        [SerializeField] private int numberOfSplitSlimes = 4;
        [SerializeField] private float playerSlimeLaunch = Mathf.PI / 4;
        [SerializeField] private float deployedSlimeLaunch = Mathf.PI / 2;

        //y(t) = 1/2 * At^2 + V_0 * t + H_0
        //H_0 = "Player Height"
        //V_0 = "Velocity"
        //A = "Gravity (-9.8)
        [SerializeField] private float playerHeight;
        [SerializeField] private float slimeVelocity = 5f;
        [SerializeField] private float projectileMotionHeight;

        //Slime Object itself:
        [SerializeField] private GameObject slimeObject;

        private void Start()
        {
            playerHeight = casterEntity.CharacterController.height;
        }

        private void FixedUpdate()
        {
            projectileMotionHeight = .5f * casterEntity.PhysicsConfig.Gravity * (Time.fixedDeltaTime) * (Time.fixedDeltaTime) + slimeVelocity * (Time.fixedDeltaTime) + playerHeight;
        }

        private void LaunchSlimes()
        {

            for (int i = 0; i < numberOfSlimes; i++)
            {
                //Creating a Slime At Each Position in Unit Circle:
                float angle = i * playerSlimeLaunch;
                Vector3 spawnPosiion = casterEntity.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                GameObject slimeProjectile = Instantiate(slimeObject, spawnPosiion, Quaternion.identity);
                Rigidbody rb = slimeProjectile.GetComponent<Rigidbody>();


                //Force Application:
                if (rb != null)
                {
                    Vector3 launchVelocity = new Vector3(Mathf.Cos(angle) * slimeVelocity, projectileMotionHeight, Mathf.Sin(angle) * slimeVelocity);
                    rb.velocity = launchVelocity;
                }
            }
        }

        //Visualization of SpawnPositions for Slimes:
        private void OnDrawGizmos()
        {
            if (casterEntity == null) return;

            for (int i = 0; i < numberOfSlimes; i++)
            {
                float angle = i * (Mathf.PI / 4);
                Vector3 spawnPos = casterEntity.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(spawnPos, 0.2f);
            }
        }

        private protected override void OnSpawn()
        {
            LaunchSlimes();
        }

        private protected override void OnOnDisable()
        {
            //Resetting Any Values Changes:

        }
    }
}

