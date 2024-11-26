using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaper : Enemy
{   
    // variables for attack go here 
    // follower has a couple of them already but I dont think leaper needs all of them
    [field: Header("Leaper: Attack Settings")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    // reminder to change the value bellow if you need to
    [field: SerializeField] public Vector2Int AttackDamageRange { get; private set; } = new Vector2Int(10,15);
    private float originalRotationSpeed;

    [field: SerializeField] public float LungeDuration { get; private set; } = 2f;
    [field: SerializeField] public float HitBoxRadius { get; private set; } = 2f;
    [field: SerializeField] public GameObject HitBoxLocation { get; private set; }
    [field: SerializeField] public LayerMask HitLayer { get; private set; }
    //[field: SerializeField] public float LeaperJumpForce { get; private set; }
    [field: SerializeField] public float LeapAttackHeight { get; private set; } = 2f;
    [field: SerializeField] public float LeapAttackDuration { get; private set; } = 0.75f;



    [field: Header("Leaper: Hop Settings")]
    [field: SerializeField] public int HopCount { get; private set; } = 2;
    [field: SerializeField] public float HopDistance { get; private set; } = 5f;
    [field: SerializeField] public float HopDuration { get; private set; } = .5f;
    [field: SerializeField] public float HopHeight { get; private set; } = .75f;


    [field: SerializeField] private Rigidbody rb;

    //Cheng - I putted the 5 variables here b/c error I got in patrol state, so I can resume testing, as can be seen
    //they are not serialized or properly declared. I assume that the patrol team will do more on these.
    public Vector2 PatrolIntervalDurationRange;
    public Vector2 PatrolRadiusRange;
    public float PatrolJumpPrepareTime;
    public float PatrolJumpHeight;
    public float PatrolJumpDuration;
    //- Cheng

    // NOTE: everything was happening to fast initially added a timer to slow and debug
    // for fun though take these off lol its funny af 
    [field: Header("Leaper Debug Settings")]
    public float debugTimerDuration = 0;
    public float debugTimer = 5f;

    
    // add all states here 
    // add as they get created

    #region States

    public LeaperAttackState LeaperAttackState{ get; private set; }
    public LeaperHopState LeaperHopState{ get; private set; }
    public LeaperChaseState LeaperChaseState{ get; private set; }
    public LeaperPatrolState LeaperPatrolState{ get; private set; }
    
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
        SetDefaultState(LeaperPatrolState);
        Debug.Log("Starting Leaper");
    } 

    private protected override void OnUpdate()
    {
        base.OnUpdate();
        if(CurrentState == EnemyChaseState)
        {
            ForceChangeState(LeaperPatrolState);
        }
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

        LeaperAttackState = new LeaperAttackState(this);
        LeaperHopState = new LeaperHopState(this);
        LeaperChaseState = new LeaperChaseState(this);
        LeaperPatrolState = new LeaperPatrolState(this);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(HitBoxLocation.transform.position, HitBoxRadius);
    }
    public void CheckForHits()
    {
        Collider[] hitColliders = Physics.OverlapSphere(HitBoxLocation.transform.position, HitBoxRadius, HitLayer);

        foreach (var hitCollider in hitColliders)
        {
            Entity hitEntity = hitCollider.GetComponent<Entity>();

            if (hitEntity != null)
            {
                if (hitEntity.Team != Team)
                {
                    //Damage Player
                    Debug.Log("Player Hit");
                    ChangeState(EntityEmptyState);
                }
            }
        }
    }

    public Tween TweenLeap(Vector3 leapDestination, float leapDuration, float leapHeight)
    {
        Vector3 startPoint = rb.position;
        Vector3 endPoint = leapDestination;
        Vector3 midPoint = (startPoint + endPoint) / 2;
        midPoint.y += leapHeight;
        Vector3[] path = { startPoint, midPoint, endPoint };
        return rb.DOPath(path, leapDuration, PathType.CatmullRom).SetEase(Ease.Linear);
    }
}
