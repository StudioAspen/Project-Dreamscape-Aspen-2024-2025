using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackState : SlimeBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public LayerMask SlimeAttackLayerMask { get; private set; }
    [field: SerializeField] public float AttackContactDamageMultiplier {get; private set;} = 1.5f;
    [field: SerializeField] public float RegularContactDamagePercent {get; private set;} = 100f;
    

    // CHANGE SCALE INCREMENT TO CHANGE GROWTH SPEED 
    [field: SerializeField] public float scaleIncrement {get; private set;} = 0.1f;

    [field: SerializeField] public float attackCooldown {get; private set;} = 2f;
    [field: SerializeField] public float shrinkCoolDown {get; private set;} = 2f;
    

    private float maxRadius = 0f;
    private float currentScale = 0f;
    // private float startScale = 0f;
    private float shrinkTimer;
    private Entity rememberedTarget;


    private List<Entity> entitiesHitByCurrentAttack = new List<Entity>();
    
    public void AssignCurrentRememberedTarget(Entity target)
    {
        rememberedTarget = target;
    }

    public override void OnEnter()
    {
        // slime.TransitionToAnimation("");
        currentScale = slime.startScale;
        maxRadius = slime.startScale * 2;

        slime.SetSpeedModifier(0f);
        entitiesHitByCurrentAttack.Clear();

        StartCoroutine(ScaleSlime());
        // StartCoroutine(ScaleOverTime());
    }

    public override void OnExit()
    {
        Debug.Log("exit");
        currentScale = slime.startScale;
        rememberedTarget = null;
    }

    IEnumerator ScaleSlime()
    {
        if(slime.hasAttacked)
        {
            yield return new WaitForSeconds(attackCooldown);
            slime.hasAttacked = false;
        }
        if (!slime.hasAttacked)
        {
            
            while(slime.transform.localScale.x < maxRadius)
            {
                yield return new WaitForSeconds(0.1f); // Wait for colliders to overlap
                slime.CheckCollisions(AttackContactDamageMultiplier, ref entitiesHitByCurrentAttack);
                
                currentScale += scaleIncrement;
                
                slime.transform.localScale = Vector3.one * currentScale;
                yield return null;
            }

           // set exact scale after so scale isnt off slightly
            slime.transform.localScale = new Vector3(maxRadius, maxRadius, maxRadius);

            yield return new WaitForSeconds(shrinkCoolDown);
            while(slime.transform.localScale.x > slime.startScale)
            {
                slime.transform.localScale -= Vector3.one * 1 * Time.deltaTime;
                yield return null;
            }

            // set exact scale after so scale isnt off slightly
            slime.hasAttacked = true;
            slime.transform.localScale = new Vector3(slime.startScale,slime.startScale,slime.startScale);
            slime.ChangeState(slime.SlimeWanderState);
        }
    }

    
    public override void OnUpdate()
    {
        base.OnUpdate();

        
    }
}
