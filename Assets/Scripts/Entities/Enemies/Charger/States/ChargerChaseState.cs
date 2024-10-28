using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerChaseState : EnemyChaseState
{
    // Going to be making comments just in case anybody needs it
    /// Initial Setup for any enemy state script
    private Charger charger;

    public ChargerChaseState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }
    // initial setup end

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

        // if no target go idle
        if (charger.Target == null)
        {
            charger.ChangeState(charger.EnemyIdleState);
            return;
        }

        // looks for target and sets what direction to face if in radius change to attack state
        // if (charger.Distance(charger.Target) < charger.AttackRange)
        // {
        //     Vector3 attackDir = charger.Target.transform.position - charger.transform.position;
        //     charger.ChargerAttackState.SetAttackDirection(attackDir);
        //     charger.ChangeState(charger.ChargerAttackState);
        //     return;
        // }

        // determines if it can even target the player
        if (charger.Target.TryGetComponent(out Player player))
        {
            if (player.NearbyEntities.Count > 0)
            {
                bool qualifiedToChase = false;
                
                // if charger within range change qualified to chase to true
                // for (int i = 0; i < Mathf.Min(charger.CircleEntityCountThreshold, player.NearbyEntities.Count); i++)
                // {
                //     if (player.NearbyEntities[i].gameObject == charger.gameObject)
                //     {
                //         qualifiedToChase = true;
                //     }
                // }

                // if charger not qualified to chase
                if (!qualifiedToChase)
                {   
                    // check if within radius
                    // if (charger.Distance(charger.Target) < charger.MaxCircleRadius)
                    // {
                    //     // follower changes to circle state
                    //     // but I assume we will have it change to the attack close state
                    // }
                    // presumably the if statement above will change to some other condition
                    // in order to have both attack(close) and attack(far)
                    // possible else if bellow for a minimum circle radius
                }
            }
        }
    }
    
    public override void FixedUpdate()
    {

    }
}
