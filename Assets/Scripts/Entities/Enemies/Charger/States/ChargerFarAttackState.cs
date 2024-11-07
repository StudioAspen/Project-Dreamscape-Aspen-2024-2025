using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerFarAttackState : EnemyBaseState
{
    private Charger charger;

    private float currentSpeed;

    private float chargeTime;
    private float slowdownTime;

    private bool isCharging = false;
    private bool isSlowingDown = false;

    private LayerMask hitLayer;

    public ChargerFarAttackState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter() //when state is switched to.
    {
        isCharging = false;
        isSlowingDown = false;

        /// Inaccessible due to its protection level Unity error
        // charger.AssignTarget(); 

        if (enemy.Target != null)
        {
            Debug.Log("Setting Charge!");
            SetCharge();
        }  
    }

    public override void OnExit() //when state is switched out.
    {
        isCharging = false;
        isSlowingDown = false;
    }

    public override void Update()
    {
        if (charger.Target == null)  
        {
            Debug.Log("No Target in range!");
            charger.ChangeState(charger.ChargerFarAttackState); //will be set to default state (goes into same state for testing).
            return;
        }

        if (isCharging || isSlowingDown)
        {
            CheckForHits();
        }

        if (isCharging)
        {
            Debug.Log("Charging!");
            StartCharging();
        }

        if (isSlowingDown)
        {
            Debug.Log("Slowing Down!");
            StartSlowingDown();
        }
    }

    public override void FixedUpdate()
    {
        if (charger.Target == null)
        {
            charger.ChangeState(charger.ChargerFarAttackState); //will be set to default state (goes into same state for testing).
            return;
        }

        //while charging checks moves towards player.
        if (isCharging || isSlowingDown)
        {
            Vector3 moveDirection = (enemy.Target.transform.position - enemy.transform.position).normalized;
            /// Enemy does not contain a def for Rigidbody error on Unity
            // enemy.RigidBody.MovePosition(enemy.transform.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
        }
    
    }

    private void SetCharge()
    {
        isCharging = true;

        currentSpeed = 0f; 
        chargeTime = Time.time; //remains constant. 
    }

    private void StartCharging()
    {
        float elapsedChargeTime = Time.time - chargeTime;

        if (elapsedChargeTime <= charger.ChargeDuration)
        {
            //Speed will be from 0 to ChargeSpeed, in the length of the charge duration. 
            currentSpeed = Mathf.Lerp(0, charger.ChargeSpeed, (Time.time - chargeTime) / charger.ChargeDuration);
        }
        else
        {
            isCharging = false;
            Debug.Log("Setting Slowdown!");
            SetSlowDown();
        } 
    }

    private void SetSlowDown()
    {
        isSlowingDown = true;

        currentSpeed = charger.ChargeSpeed; 
        slowdownTime = Time.time; //remains constant.
    }

    private void StartSlowingDown()
    {
        float elapsedSlowTime = Time.time - slowdownTime;

        if (elapsedSlowTime <= charger.SlowDownDuration)
        {
            //Speed will be from ChargeSpeed to 0, in the length of the slowdown duration.
            currentSpeed = Mathf.Lerp(currentSpeed, 0, (Time.time - slowdownTime) / charger.SlowDownDuration);
        }
        else
        {
            isSlowingDown = false;
            //Enter Wind-down State.
            Debug.Log("Entering Wind-down State!");
        }
    }

    private void CheckForHits()
    {
        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, charger.HitboxRadius, hitLayer);

        foreach (var hitCollider in hitColliders)
        {
            Entity hitEntity = hitCollider.GetComponent<Entity>();
            if(hitEntity == null) continue;

            //in the case no entity was hit, but hit a wall.
            if (hitCollider.gameObject.layer == LayerMask.GetMask("Wall"))
            {
                //take self damage.
                
                //tiny knockback.
                Vector3 knockbackDirection = (charger.transform.position - hitCollider.transform.position).normalized;
                ApplyKnockback(charger, knockbackDirection, charger.KnockbackForce);
                

                //enter dazed state.
            }

            //if both entities are on the same team [enemy]
            if (hitEntity.Team == enemy.Team) 
            {
                //knockback other enemy in charge direction.
                Vector3 knockbackDirection = (hitEntity.transform.position - charger.transform.position).normalized;
                ApplyKnockback(hitEntity, knockbackDirection, charger.KnockbackForce); 

            }
            //else if not on the same team [player]
            else
            {
                //damage player.

                //knockback to player.
                Vector3 knockbackDirection = (hitEntity.transform.position - charger.transform.position).normalized;
                ApplyKnockback(hitEntity, knockbackDirection, charger.KnockbackForce);

                //enter wind-down state.
            }
        }
    }

    private void ApplyKnockback(Entity target, Vector3 direction, float force)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();

        if (targetRigidbody != null)
        {
            targetRigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    /* -[OLD LOGIC]-

    [Header("Charging Radius")]
    public float chargingProcRadius;    //radius for entering charge state.
    public float chargingEndRadius;     //radius for within players range to stop tracking.
    
    [Header("Charging Adjustments")]
    public float chargeSpeed;
    public float slowingDownDuration;       //time in seconds for how long the charger moves after tracking stops.
    public float rotationSpeed;

    private Transform player;
    private Vector3 targetPosition;      

    private bool isCharging = false;          
    private bool isSlowingDown = false;       

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        //variable made for reusability.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //if distance between charger and player is less than "chargingProcRadius" and is not yet charging or slowing down then:
        if (distanceToPlayer <= chargingProcRadius && !isCharging && !isSlowingDown)
        {
            StartCoroutine(Charge());
        }

        //if distance between charger and player is less than "chargingEndRadius" and is charging, but not yet slowing down then:
        if (distanceToPlayer <= chargingEndRadius && isCharging && !isSlowingDown)
        {
            StopCoroutine(Charge());
            StartCoroutine(SlowingDown());
        }
    }

    private IEnumerator Charge()
    {
        isCharging = true;

        while (isCharging)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            //charging enemy looking towards player while charging.
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            //charging logic.
            transform.position += direction * chargeSpeed * Time.deltaTime;

            //moves each frame.
            yield return null; 
        }
    }

    private IEnumerator SlowingDown()
    {
        isCharging = false;
        isSlowingDown = true;

        Vector3 forwardDirection = transform.forward;
        float timer = 0f;

        //continues moving foward for the specified "slowingDownDuration". 
        while (timer < slowingDownDuration)
        {
            transform.position += forwardDirection * chargeSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isSlowingDown = false; 
    }

    private void OnDrawGizmosSelected()
    {
        //visual for "chargingProcRadius",
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chargingProcRadius);

        //visual for "chargingEndRadius",
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargingEndRadius);
    }

    */
}
