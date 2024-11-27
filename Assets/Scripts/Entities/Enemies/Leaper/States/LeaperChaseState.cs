using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaperChaseState : EnemyChaseState
{
    private Leaper leaper; // Reference to the specific Leaper enemy using this state
    private float safeDistanceForLeap = 2f; // Minimum distance from player before preparing to leap or hop back

    /// <summary>
    /// Constructor initializes the Leaper enemy that uses this chase state.
    /// </summary>
    /// <param name="enemy">The Leaper enemy associated with this chase state.</param>
    public LeaperChaseState(Leaper enemy) : base(enemy)
    {
        leaper = enemy;
    }

    /// <summary>
    /// Called when the LeaperChaseState is first entered.
    /// Sets up the animation and speed modifier for the Leaper's chase behavior.
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        leaper.DefaultTransitionToAnimation("LeaperMovement"); // Switch to movement animation for chase behavior
        leaper.SetSpeedModifier(0.8f); // Slightly reduce speed to allow for reaction time during hops
    }

    /// <summary>
    /// Called when the LeaperChaseState is exited.
    /// Inherits cleanup logic from the base class.
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// Called once per frame to update the chase behavior.
    /// Checks the distance to the target and decides whether to continue chasing
    /// or transition to a hop/leap if within the safe distance.
    /// </summary>
    public override void Update()
    {
        base.Update();

        // If there is no target, transition to patrol state
        // if (leaper.Target == null)
        // {
        //     leaper.ChangeState(leaper.LeaperPatrolState);
        //     return;
        // }

        // Calculate the distance to the player or target
        float distanceToTarget = leaper.Distance(leaper.Target);

        // Move towards the player if the target is beyond the safe distance threshold
        if (distanceToTarget > safeDistanceForLeap)
        {
            // Debug.Log(leaper.Distance(leaper.Target));
            leaper.SetDestination(leaper.Target.transform.position, true);
        }
        else
        {
            // Debug.Log(leaper.Distance(leaper.Target));

            // If within the safe distance, prepare for hop or leap back
            leaper.ChangeState(leaper.LeaperHopState);
        }
    }

    /// <summary>
    /// FixedUpdate for physics-based updates. Left empty as no physics are handled here.
    /// Can be removed if not required in derived classes.
    /// </summary>
    public override void FixedUpdate()
    {
        // Consider removing or clarifying this if no physics actions are needed in FixedUpdate
    }
}
