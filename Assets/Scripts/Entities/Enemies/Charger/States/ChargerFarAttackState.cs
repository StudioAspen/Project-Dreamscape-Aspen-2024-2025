using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class ChargerFarAttackState : EnemyBaseState {
    private Charger charger;

    [Header("Charging Radius")]
    public float chargingProcRadius;    //radius for entering charge state.
    public float chargingEndRadius;     //radius for within players range to stop tracking.

    [Header("Charging Adjustments")]
    public float chargeSpeed;
    public float slowingDownDuration;       //time in seconds for how long the charger moves after tracking stops.
    public float rotationSpeed;
    private bool isCharging = false;
    private bool rotationLocked = false;
    private float ChargerFocusRotNumIntervals = 100f;
    Vector3 chargeDestination;

    private LayerMask hitLayer;

    private Coroutine chargeCoroutine;
    private Coroutine chargeAccelerateCoroutine;

    public ChargerFarAttackState(Charger enemy) : base(enemy) {
        charger = enemy;
    }

    public override void OnEnter() //when state is switched to.
    {
        if (enemy.Target == null) {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }

        charger.SetSpeedModifier(0f);

        isCharging = false;
        rotationLocked = false;


        /// Inaccessible due to its protection level Unity error
        // charger.AssignTarget(); 

        if (enemy.Target != null) {
            charger.SetDestination(charger.Target.transform.position, false); // set second param to false to enable lookat
            Debug.Log("Setting Charge!");
            SetCharge();
        }
    }

    public override void OnExit() //when state is switched out.
    {
        isCharging = false;
        rotationLocked = false;
        charger.SetDestination(charger.transform.position, true);
        if (chargeCoroutine != null) {
            charger.StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
        }
        if (chargeAccelerateCoroutine != null) {
            charger.StopCoroutine(chargeAccelerateCoroutine);
            chargeAccelerateCoroutine = null;
        }
    }

    public override void Update() {
        if (isCharging) {
            charger.SetDestination(chargeDestination, true);
            return;
        }

        if (rotationLocked) return;

        if (charger.Target != null) {
            charger.LookAt(charger.Target.transform.position);
        }
    }

    public override void FixedUpdate() {


        if (charger.Target == null) {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }


       

    }


    private void SetCharge() {
        chargeCoroutine = charger.StartCoroutine(Charge());
    }


    private IEnumerator Charge() {

        // Give x amount of time for charger to decide angle to charge rotation onto player
        yield return new WaitForSeconds(charger.TargetFocusTime);
        rotationLocked = true;
        // Freeze rotation before suddenly charging
        //yield return new WaitForSeconds(1f); // change to sudden charge delay



        Debug.Log("CHARGE!!!");
        chargeDestination = FindFurthestGroundPoint();
        isCharging = true;

        charger.SetSpeedModifier(charger.ChargeSpeedMin);
        chargeAccelerateCoroutine = charger.StartCoroutine(ChargeAccelerate());

        // Charge!!! Charger runs blindly in a straight line until it hits something

        float startTime = Time.time;

        yield return new WaitUntil(() => Time.time - startTime >= charger.MaxChargeDuration || CloseEnoughToPoint(chargeDestination));

        

        charger.ChangeState(charger.ChargerDazedState);
        yield return null;

    }

    private IEnumerator ChargeAccelerate() {
        int numSpeedSteps = 75;
        float speedStep = (charger.ChargeSpeedMax - charger.ChargeSpeedMin) / numSpeedSteps;
        for (float i = charger.ChargeSpeedMin; i < charger.ChargeSpeedMax; i += speedStep) {
            charger.SetSpeedModifier(i);
            yield return new WaitForSeconds(charger.ChargeAccelerationInterval);
        }

    }


    private Vector3 FindFurthestGroundPoint() {
        
        float stepSize = .5f;
        float downRayDistance = 10f;
        float navMeshSampleRadius = .2f;
        float chargerHeight = 1.65f;
       

        Vector3 origin = charger.transform.position + charger.transform.forward;
        Vector3 furthestGroundPoint = origin;
        for (float distance = 0; distance < charger.MaxChargeDistance; distance += stepSize) {
            Vector3 forwardPoint = origin + (Vector3.up * chargerHeight) + (charger.transform.forward * distance);
            RaycastHit rayHit = new();

            bool raycastSuccess = Physics.Raycast(forwardPoint, Vector3.down, out rayHit, downRayDistance, LayerMask.GetMask("Ground"));
            charger.StartCoroutine(CreateDebugPoint(forwardPoint, raycastSuccess ? Color.green : Color.red));
            Debug.DrawRay(forwardPoint, Vector3.down * downRayDistance, raycastSuccess ? Color.green : Color.red, 5f);

            bool navmeshSampleSuccess = NavMesh.SamplePosition(rayHit.point, out _, navMeshSampleRadius, NavMesh.AllAreas);
            charger.StartCoroutine(CreateDebugPoint(forwardPoint + Vector3.up, navmeshSampleSuccess ? Color.white : Color.black));

            
            bool reachAbsoluteEnd = navmeshSampleSuccess && !raycastSuccess;

            bool navmeshFail = !navmeshSampleSuccess;
            charger.StartCoroutine(CreateDebugPoint(rayHit.point, navmeshSampleSuccess ? Color.green : Color.red));


            if (navmeshFail) {
                break;
            } else {
                furthestGroundPoint = (rayHit.point == Vector3.zero ) ? furthestGroundPoint : rayHit.point;
            }
        }

        
        Debug.DrawRay(furthestGroundPoint, Vector3.up * 10, Color.yellow, 5f);

        return furthestGroundPoint;

    }


    private bool CloseEnoughToPoint(Vector3 point) {
        return (charger.transform.position - point).magnitude < .33f;
    }

    private IEnumerator CreateDebugPoint(Vector3 pos, Color color) {
        // show destination point for debugging
        GameObject hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hitPoint.transform.localScale = new Vector3(.2f, .2f, .2f);
        hitPoint.transform.position = pos;
        hitPoint.GetComponent<Collider>().enabled = false;
        hitPoint.GetComponent<Renderer>().material.color = color;
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(hitPoint);
        yield return null;
    }


}