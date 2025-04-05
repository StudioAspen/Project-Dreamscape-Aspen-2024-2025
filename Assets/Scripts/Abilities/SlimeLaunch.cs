using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class SlimeLaunch : CastedAbility, IPoolableObject
    {
        //Should Not be Accessable as a setting's Change within the Scriptable Object:
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
        [SerializeField] private float slimeVelocity; //Customizable in the Scriptable Object

        [SerializeField] private float playerHeight;
        [SerializeField] private float projectileMotionHeight;

        //Slime Object itself: "Called in Scriptable Object"
        [SerializeField] private GameObject slimeObject;


        //Setting Variable Values given from the Scriptable Object:
        public void SetSlimePrefab(GameObject slime)
        {
            this.slimeObject = slime;
        }
        public void SetVelocity(float velocity)
        {
            this.slimeVelocity = velocity;
        }


        //Start by Getting Value for Player Height:
        private void Start()
        {
            playerHeight = casterEntity.CharacterController.height;
        }

        //As Time Changes -> The Height Changes creating the Projectile Motion Effect:
        private void FixedUpdate()
        {
            projectileMotionHeight = .5f * casterEntity.PhysicsConfig.Gravity * (Time.fixedDeltaTime) * (Time.fixedDeltaTime) + slimeVelocity * (Time.fixedDeltaTime) + playerHeight;
        }

        //Triggered When the Memory Ability is Activated:
        // - Gets the [8] Positions to Instantiate The Slime
        // - Then Instantiate Them while also giving a Rb Component
        // - Then With the Rb Apply Force
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

        //Called When the Memory Ability Starts:
        private protected override void OnSpawn()
        {
            LaunchSlimes();
        }

        //Called When the Memory Ability Ends:
        private protected override void OnOnDisable()
        {
            //Resetting Any Values Changes:

        }



        // Logic Implemented for When Enemy Entites collide With the Slime Prefab:
        private void CheckForEnemyHit(Collider other)
        {
            SecondLaunchSlimes();
        }


        //Triggered When The Launched Slime Prefab Collides With an Enemy:
        // - Gets the [4] Positions to Instantiate The Slime
        // - Then Instantiate Them while also giving a Rb Component
        // - Then With the Rb Apply Force
        private void SecondLaunchSlimes()
        {
            for (int i = 0; i < numberOfSplitSlimes; i++)
            {
                //Creating a Slime At Each Position in Unit Circle:
                float angle = i * deployedSlimeLaunch;
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


    }
}

