using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dreamscape.Abilities
{
    public class SlimeLaunch : CastedAbility, IPoolableObject
    {
        //Should Not be Accessable as a setting's Change within the Scriptable Object:
        [Header("Settings")] 
        private float slimeIndex = 1; //Index used to determine if slime can split

        private int numberOfSlimes = 8;
        private int numberOfSplitSlimes = 4;

        private float playerSlimeLaunch = Mathf.PI / 4;
        private float deployedSlimeLaunch = Mathf.PI / 2;


        //y(t) = 1/2 * At^2 + V_0 * t + H_0
        //H_0 = "Player Height"
        //V_0 = "Velocity"
        //A = "Gravity (-9.8)
        [SerializeField] private float slimeVelocity; 
        [SerializeField] private float arcHeight; 
        [SerializeField] private float playerHeight;
        [SerializeField] private float projectileMotionHeight;

        [SerializeField] private GameObject slimeObject;
        [SerializeField] private GameObject slimeTrail;
        [SerializeField] private LayerMask slimeTrailLayer;
        [SerializeField] private float raycastDistance = 1.5f;
        [SerializeField] private float trailSpawnRate = 0.1f;
        private float trailTimer = 0f;


        //Setting Variable Values given from the Scriptable Object:
        public void SetSlimePrefab(GameObject slime)
        {
            this.slimeObject = slime;
        }
        public void SetSlimeTrail(GameObject trial)
        {
            this.slimeTrail = trial;
        }
        public void SetVelocity(float velocity)
        {
            this.slimeVelocity = velocity;
        }
        public void SetArc(float arc)
        {
            this.arcHeight = arc;
        }
        public void SetIgnoredLayers(LayerMask layers)
        {
            slimeTrailLayer = layers;
        }


        //Start by Getting Value for Player Height:
        private void Start()
        {

        }

        private void Update()
        {
            trailTimer += Time.deltaTime;
            if (trailTimer >= trailSpawnRate)
            {
                trailTimer = 0f;
                CastTrailRayFromSlimes();
            }
        }

        private void CastTrailRayFromSlimes()
        {
            GameObject[] slimes = GameObject.FindGameObjectsWithTag("Launchable Slime");

            foreach (GameObject slime in slimes)
            {
                Vector3 origin = slime.transform.position;
                Ray ray = new Ray(origin, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, slimeTrailLayer))
                {
                    Instantiate(slimeTrail, hit.point, Quaternion.identity);
                }
            }
        }

        //As Time Changes -> The Height Changes creating the Projectile Motion Effect:
        private void FixedUpdate()
        {

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
                    Vector3 horizontalDir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)).normalized;
                    Vector3 launchVelocity = horizontalDir * slimeVelocity + Vector3.up * arcHeight;
                    rb.velocity = launchVelocity;
                }
            }
        }

        //Called When the Memory Ability Starts:
        private protected override void OnSpawn()
        {
            Debug.Log("ABILITY STARTED!!!");
            LaunchSlimes();
        }

        //Called When the Memory Ability Ends:
        private protected override void OnOnDisable()
        {
            //Resetting Any Values Changes:
        }

        //Triggered When The Launched Slime Prefab Collides With an Enemy:
        // - Gets the [4] Positions to Instantiate The Slime
        // - Then Instantiate Them while also giving a Rb Component
        // - Then With the Rb Apply Force
        // *** SPAWN POSITION SHOULD NOT BE CASTERENTITY --> SHOULD BE SLIME THAT WAS COLLIDED WITH:
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

