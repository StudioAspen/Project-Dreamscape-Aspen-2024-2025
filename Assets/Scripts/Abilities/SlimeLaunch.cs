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
        private int numberOfSlimes = 8;
        private int numberOfSplitSlimes = 4;

        private float playerSlimeLaunch = Mathf.PI / 4;
        private float deployedSlimeLaunch = Mathf.PI / 2;


        //y(t) = 1/2 * At^2 + V_0 * t + H_0
        //H_0 = "Player Height"
        //V_0 = "Velocity"
        //A = "Gravity (-9.8)
        [SerializeField] private float slimeIndex;
        [SerializeField] private float slimeVelocity; 
        [SerializeField] private float projectileMotionHeight;

        [SerializeField] private GameObject slimeObject;
        [SerializeField] private GameObject slimeTrail;
        [SerializeField] private LayerMask slimeTrailLayer;
        [SerializeField] private float raycastDistance = 1.5f;
        [SerializeField] private float trailSpawnRate = 0.1f;
        [SerializeField] private float slimeLifeSpan;
        [SerializeField] private float trailLifeSpan;


        private float trailTimer = 0f;


        //Setting Variable Values given from the Scriptable Object:
        public void SetSlimeIndex(float index)
        {
            this.slimeIndex = index;
        }
        public void SetSlimePrefab(GameObject slime)
        {
            this.slimeObject = slime;
        }
        public void SetSlimeTrail(GameObject trial)
        {
            this.slimeTrail = trial;
        }
        public void SetIgnoredLayers(LayerMask layers)
        {
            this.slimeTrailLayer = layers;
        }
       



        //Start by Getting Value for Player Height:
        private void Start()
        {
        }

        //Update used to control the Spawning Period For the Slime Trail:
        private void Update()
        {
            trailTimer += Time.deltaTime;
            if (trailTimer >= trailSpawnRate)
            {
                trailTimer = 0f;
                SlimeTrail();
            }

            GameObject[] trails = GameObject.FindGameObjectsWithTag("Slime Trail");
            foreach (GameObject trail in trails)
            {
                Destroy(trail, trailLifeSpan);
            }
        }

        //Gets Refrence to All Slimes On the Scene -> Instantiates the Slime Trail Below them if the Ray Hit the Specified Layer Mask:
        private void SlimeTrail()
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


        //Collision Detection To Make Sure Slimes Split Properly:
        // - 
        private void OnCollisionEnter(Collision collision)
        {
            // Check if the slime's collision is not with something in the slimeTrailLayer
            if ((slimeTrailLayer.value & (1 << collision.gameObject.layer)) == 0)
            {               
                SecondLaunchSlimes(this.gameObject); 
            }
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

                Destroy(slimeProjectile, slimeLifeSpan); //Destroy Initial after Specified time
            }
        }


        //Triggered When The Launched Slime Prefab Collides With an Enemy:
        // - Gets the [4] Positions to Instantiate The Slime
        // - Then Instantiate Them while also giving a Rb Component
        // - Then With the Rb Apply Force
        private void SecondLaunchSlimes(GameObject collidedSlime)
        {
            if (slimeIndex > 0)
            {
                slimeIndex--;

                for (int i = 0; i < numberOfSplitSlimes; i++)
                {
                    //Creating a Slime At Each Position in Unit Circle:
                    float angle = i * deployedSlimeLaunch;
                    Vector3 spawnPosiion = collidedSlime.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                    Destroy(collidedSlime); //Destroy Initial On Split
                    GameObject slimeProjectile = Instantiate(slimeObject, spawnPosiion, Quaternion.identity);
                    Rigidbody rb = slimeProjectile.GetComponent<Rigidbody>();


                    //Force Application:
                    if (rb != null)
                    {
                        Vector3 launchVelocity = new Vector3(Mathf.Cos(angle) * slimeVelocity, projectileMotionHeight, Mathf.Sin(angle) * slimeVelocity);
                        rb.velocity = launchVelocity;
                    }

                    Destroy(slimeProjectile, slimeLifeSpan);
                }
            }    
        }


        //Called When the Memory Ability Starts:
        private protected override void OnSpawn()
        {
            trailTimer = 0f;
            Debug.Log("ABILITY STARTED!!!");
            LaunchSlimes();
            DestroyAndRelease();
        }

        //Called When the Memory Ability Ends:
        private protected override void OnOnDisable()
        {
            //Resetting Any Values Changes:
        }
    }
}

