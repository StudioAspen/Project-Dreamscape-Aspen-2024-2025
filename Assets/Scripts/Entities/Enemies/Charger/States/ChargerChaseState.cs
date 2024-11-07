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
        base.Update();

        if (charger.Target == null)
        {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }

        // within range attack/switch to attack state
        // for charger have another if statement to check if too close for
        // long range attack

        if (charger.Distance(charger.Target) < charger.AttackRange)
        {

            // Vector3 attackDir = charger.Target.transform.position - charger.transform.position;
            // charger.ChargerFarAttackState.SetAttackDirection(attackDir);
            charger.ChangeState(charger.ChargerFarAttackState);
        }

        /// Not sure if we want to circle but ill write it anyway

        if (charger.Distance(charger.Target) < charger.CircleRadius)
        {
            CheckCanCircle();
        }
    }

    private void CheckCanCircle()
    {
        if (charger.Target.TryGetComponent(out Player player))
        {
            List<Charger> playerNearbyChargers = player.GetNearbyEntitiesByType<Charger>(charger.CircleRadius + 1f);

            foreach (Charger c in new List<Charger>(playerNearbyChargers))
            {
                if(c.CurrentState == c.EntityDeathState) playerNearbyChargers.Remove(c);
            }

            // playerNearbyChargers = playerNearbyChargers.Take(charger.CircleChargerCountThreshold).ToList();

            if (playerNearbyChargers.Contains(charger)) return;
            // charger.ChangeState(charger.ChargerCircleState);
        }
    }

    public override void FixedUpdate()
    {

    }

}
