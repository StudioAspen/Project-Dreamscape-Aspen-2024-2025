using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaper : Enemy
{   
    // variables for attack go here 
    // follower has a couple of them already but I dont think leaper needs all of them
    [field: Header("Leaper: Attack Settings")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    // reminder to change the value bellow if you need to
    [field: SerializeField] public Vector2Int AttackDamageRange { get; private set; } = new Vector2Int(10,15);


    // add all states here 
    // add as they get created
    #region States

    // public LeaperAttackState LeaperAttackState{ get; private set; }
    // public LeaperHopState LeaperHopState{ get; private set; }
    // public LeaperChaseState LeaperChaseState{ get; private set; }
    
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();
        // set start state
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    private protected override void OnStart()
    {
        base.OnStart();
    } 

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        // Initialize new states here 
        // currently only 3 
        // add more as they get created

        // LeaperAttackState = new LeaperAttackState(this);
        // LeaperHopState = new LeaperHopState(this);
        // LeaperChaseState = new LeaperChaseState(this);


    }
}
