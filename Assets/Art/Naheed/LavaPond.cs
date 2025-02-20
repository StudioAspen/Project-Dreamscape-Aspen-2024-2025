using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPond : MonoBehaviour
{

    [SerializeField] private StatusEffectSO burnStatusEffect;

    private void OnTriggerStay(Collider collision)
    {
        EntityStatusEffector entityStatusEffector = collision.gameObject.GetComponent<EntityStatusEffector>(); 
        if (entityStatusEffector == null)
            if (collision.gameObject.TryGetComponent<EntityStatusEffector>(out EntityStatusEffector affector))
                entityStatusEffector = affector; 

        if (entityStatusEffector != null)  
            entityStatusEffector.ApplyStatusEffect(burnStatusEffect, collision.gameObject); 
    }
}
   
