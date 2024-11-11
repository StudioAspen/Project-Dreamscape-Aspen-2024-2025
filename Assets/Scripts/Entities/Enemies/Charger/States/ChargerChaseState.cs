using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerChaseState : EnemyChaseState
{
    private Charger charger;

    public ChargerChaseState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        // setdestination and lookat cannot coexist if setdestination's second param is true!




        if (charger.Target == null)
        {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }

        if (charger.Target != null)
        {
            enemy.SetDestination(enemy.Target.transform.position, true);
        }

        // within range attack/switch to attack state
        // for charger have another if statement to check if too close for
        // long range attack

        if (charger.Distance(charger.Target) < charger.ChargingProcRadius)
        {

            // Vector3 attackDir = charger.Target.transform.position - charger.transform.position;
            // charger.ChargerFarAttackState.SetAttackDirection(attackDir);
            RaycastHit hit;
            bool inLineOfSight = ConeRaycast(64);



            if (inLineOfSight)
            {
                charger.ChangeState(charger.ChargerFarAttackState);
            }


        }

        /// Not sure if we want to circle but ill write it anyway

/*        if (charger.Distance(charger.Target) < charger.CircleRadius)
        {
            CheckCanCircle();
        }*/
    }

    private void CheckCanCircle()
    {
/*        if (charger.Target.TryGetComponent(out Player player))
        {
            List<Charger> playerNearbyChargers = player.GetNearbyEntitiesByType<Charger>(charger.CircleRadius + 1f);

            foreach (Charger c in new List<Charger>(playerNearbyChargers))
            {
                if (c.CurrentState == c.EntityDeathState) playerNearbyChargers.Remove(c);
            }

            // playerNearbyChargers = playerNearbyChargers.Take(charger.CircleChargerCountThreshold).ToList();

            if (playerNearbyChargers.Contains(charger)) return;
            // charger.ChangeState(charger.ChargerCircleState);
        }*/
    }

    private bool ConeRaycast(int numRays)
    {
        float coneAngle = 15f;
        for (int i = 0; i < numRays; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.z = Mathf.Abs(randomDirection.z);
            randomDirection = Quaternion.Euler(Random.Range(-coneAngle, coneAngle), Random.Range(-coneAngle, coneAngle), 0) * charger.transform.forward;

            Debug.DrawRay(charger.transform.position, randomDirection * 50f, Color.yellow, 1f);
            if (Physics.Raycast(charger.transform.position, randomDirection, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")) && hit.transform != charger.transform)
            {
                return true;
            }

        }
        return false;
    }

    public override void FixedUpdate()
    {

    }

}