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

    [field: Header("Leaper: Hop Settings")]
    [field: SerializeField] public int HopCount { get; private set; } = 2;
    [field: SerializeField] public float HopDistance { get; private set; } = 5f;
    [field: SerializeField] public float HopDuration { get; private set; } = .5f;
    [field: SerializeField] public float HopHeight { get; private set; } = .75f;


    // add all states here 
    // add as they get created
    #region States

    // public LeaperAttackState LeaperAttackState{ get; private set; }
    public LeaperHopState LeaperHopState{ get; private set; }
    // public LeaperChaseState LeaperChaseState{ get; private set; }

    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        // set start state
        SetStartState(LeaperHopState);
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    protected override void OnStart()
    {
        base.OnStart();
    } 

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        // Initialize new states here 
        // currently only 3 
        // add more as they get created

        // LeaperAttackState = new LeaperAttackState(this);
        LeaperHopState = new LeaperHopState(this);
        // LeaperChaseState = new LeaperChaseState(this);


    }
}
