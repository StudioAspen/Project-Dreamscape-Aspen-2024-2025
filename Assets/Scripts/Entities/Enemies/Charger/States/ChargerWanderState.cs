using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerWanderState : EnemyBaseState
{

    private Charger charger;
    private Coroutine wanderCoroutine;

    private float preRaycastOffset = 5f;
    private float raycastMaxDistance = 15f;
    private float wanderEndDistanceThreshold = 2f;

    private Vector3 wanderDestination;

    public ChargerWanderState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }
    public override void OnEnter()
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");
        charger.SetSpeedModifier(1f);
        wanderCoroutine = charger.StartCoroutine(WanderCoroutine());
        
     
    }

    public override void OnExit()
    {
        if (wanderCoroutine!= null)
        {
            charger.StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }
    }

    public override void Update()
    {
        // When charger sees player, change state to ChargerPlayerDetectedState
        if (charger.Target != null)
        {
            //charger.ChangeState(charger.ChargerPlayerDetectedState);
        }
    }

    public override void FixedUpdate() { }

 
    private IEnumerator WanderCoroutine()
    {
        // Need to check if wander point is valid, or else they will stop moving
        GoToRandomWanderPoint();
        Debug.Log("tried to go to random point");
        // wait until wandering finishes
        yield return new WaitUntil(() => CloseEnoughToPoint(wanderDestination)  );


        charger.ChangeState(charger.ChargerIdleState);
    }

    private IEnumerator CreateDebugPoint(Vector3 pos)
    {
        // show destination point for debugging
        GameObject hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //hitPoint.transform.localScale = new Vector3(.2f, .2f, .2f);
        hitPoint.transform.position = pos;
        
        yield return null;
    }


    private bool CloseEnoughToPoint(Vector3 point)
    {
        return (charger.transform.position - point).magnitude < wanderEndDistanceThreshold;
    }

    

    private void GoToRandomWanderPoint()
    {
        

        Vector3 randomPoint = (Random.insideUnitSphere * charger.WanderRadius) + charger.transform.position;
        randomPoint.y += preRaycastOffset;
        Debug.Log("Random point: " + randomPoint);
        //charger.StartCoroutine(CreateDebugPoint(randomPoint));

        // Raycast downwards to prevent charger from not wandering at all because it cannot reach
        RaycastHit raycastHit = new();
        bool success = false;
        int numTries = 5;

        for (int i = 0; i < numTries; i++)
        {
            success = Physics.Raycast(randomPoint, Vector3.down, out raycastHit, raycastMaxDistance);
            if (success) break;
              
        }

        if (!success)
        {
            Debug.Log("Failed to raycast down");
           
        }

        wanderDestination = raycastHit.point + Vector3.up;


        charger.SetDestination(wanderDestination, true);


        Debug.LogWarning(wanderDestination);

        //Debug.Log("Hit: " + raycastHit.point);
        charger.StartCoroutine(CreateDebugPoint(raycastHit.point));
        Debug.DrawRay(randomPoint, Vector3.down * 100, Color.red, 60f);


    }



}