using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChargerFarAttackState : EnemyBaseState
{
    private float chargeSpeed;
    private float currentSpeed;

    private float chargeDuration; 
    private float slowDownDuration;

    private float rotationSpeed;

    private bool isCharging = false;
    private bool isSlowingDown = false;

    private float hitboxRadius;
    private LayerMask hitLayer;

    public ChargerFarAttackState(Enemy enemy, float chargeSpeed, float slowDownDuration, float rotationSpeed, float chargeDuration) 
        : base(enemy)
    {
        this.chargeSpeed = chargeSpeed;
        this.chargeDuration = chargeDuration;
        this.slowDownDuration = slowDownDuration;
        this.rotationSpeed = rotationSpeed;
    }

    public override void OnEnter()
    {
        isCharging = false;
        isSlowingDown = false;
        enemy.AssignTarget(); 
    }

    public override void OnExit()
    {
        isCharging = false;
        isSlowingDown = false;
    }

    public override void Update()
    {
        //continuously look for a target.
        if (enemy.Target == null)  
        {
            enemy.AssignTarget(); 
        }

        float distanceToTarget = enemy.Distance(enemy.Target);

        //if distance to target is within charging range.
        if (distanceToTarget <= enemy.chargingProcRadius && !isCharging && !isSlowingDown)
        {
            StartCharging();  //[When other states are done, this is the only thing needed in update]
        }

        //while charging checks for collisions.
        if (isCharging || isSlowingDown)
        {
            CheckForHits();
        }
    }

    public override void FixedUpdate()
    {

    }

    private void StartCharging()
    {
        isCharging = true;
        currentSpeed = 0f; 

        if (enemy.Target == null) return; 

        //charging towards the target.
        DOTween.To(() => currentSpeed, x => currentSpeed = x, chargeSpeed, chargeDuration)
            .SetEase(Ease.InCubic)
            .OnUpdate(() =>
            {
                Vector3 moveDirection = (enemy.Target.transform.position - enemy.transform.position).normalized;

                enemy.transform.position += moveDirection * currentSpeed * Time.deltaTime;
            })
            .OnComplete(() => 
            {
                StartSlowingDown(); //start slowing down when charging is done.
            }
        );
    }

    private void StartSlowingDown()
    {
        isCharging = false;
        isSlowingDown = true;
        currentSpeed = chargeSpeed;

        //moves forward slowing down.
        DOTween.To(() => currentSpeed, x => currentSpeed = x, 0, slowDownDuration)
            .SetEase(Ease.OutCubic)
            .OnUpdate(() =>
            {
                enemy.transform.position += enemy.transform.forward * currentSpeed * Time.deltaTime; 
            })
            .OnComplete(() =>
            {
                isSlowingDown = false;

                //enter wind-down state.
            }
        );
    }


    private void CheckForHits()
    {
        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, hitboxRadius, hitLayer);

        foreach (var hitCollider in hitColliders)
        {
            Entity hitEntity = hitCollider.GetComponent<Entity>();
        
            if (hitEntity != null) 
            {
                //if both entities are on the same team [enemy]
                if (hitEntity.Team == enemy.Team) 
                {

                    //knockback other enemy in charge direction.

                }
                //else if not on the same team [player]
                else
                {
                    
                    //damage player.
                    //knockback to player.

                    //enter wind-down state.

                }
            }
            //in the case no entity was hit, but hit a wall
            else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Wall")) 
            {

                //take self damage.
                //tiny knockback.

                //enter dazed state.

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //visual for "chargingProcRadius".
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemy.transform.position, enemy.chargingProcRadius);

        //visual for "chargingEndRadius".
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(enemy.transform.position, enemy.chargingEndRadius);
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
