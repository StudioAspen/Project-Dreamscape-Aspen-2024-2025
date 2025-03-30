using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamscape.Abilities
{
    public class SlimeLaunch : CastedAbility, IPoolableObject
    {
        [Header("Settings")]
        [SerializeField] private float slimeIndex = 1; //Index used to determine if slime can split
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

        //Time.FixedDeltaTime 
        private void FixedUpdate()
        {
            projectileMotionHeight = .5f * casterEntity.PhysicsConfig.Gravity * (Time.fixedDeltaTime) * (Time.fixedDeltaTime) + slimeVelocity * (Time.fixedDeltaTime) + playerHeight;

        



           
        }





        private protected override void OnSpawn()
        {
            throw new System.NotImplementedException();
        }

        private protected override void OnOnDisable()
        {
            throw new System.NotImplementedException();
        }
    }

}

