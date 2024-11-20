using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leaper : Enemy
{
    [field: Header("Leaper: Patrol Settings")]
    [field: SerializeField] public Vector2 PatrolIntervalDurationRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 PatrolRadiusRange { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public float PatrolLeapHeight { get; private set; } = 2f;
    [field: SerializeField] public float PatrolLeapPrepareTime { get; private set; } = 1f;
    [field: SerializeField] public float PatrolLeapDuration { get; private set; } = .75f;

    // variables for attack go here 
    // follower has a couple of them already but I dont think leaper needs all of them
    [field: Header("Leaper: Attack Settings")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    // reminder to change the value bellow if you need to
    [field: SerializeField] public Vector2Int AttackDamageRange { get; private set; } = new Vector2Int(10,15);

    [field: SerializeField] public float LungeDuration { get; private set; } = 2f;
    [field: SerializeField] public float HitBoxRadius { get; private set; } = 2f;
    [field: SerializeField] public GameObject HitBoxLocation { get; private set; }
    [field: SerializeField] public LayerMask HitLayer { get; private set; }
    //[field: SerializeField] public float LeaperJumpForce { get; private set; } 

    private float originalRotationSpeed;


    // add all states here 
    // add as they get created
    #region States

    public LeaperAttackState LeaperAttackState{ get; private set; }
    // public LeaperHopState LeaperHopState{ get; private set; }
    // public LeaperChaseState LeaperChaseState{ get; private set; }
    public LeaperPatrolState LeaperPatrolState { get; private set; }
    
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(LeaperAttackState);

        // set start state
        SetStartState(LeaperPatrolState);

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

        SetDefaultState(LeaperAttackState);

        SetDefaultState(LeaperPatrolState);
        FinishAnimation();

        originalRotationSpeed = rotationSpeed; // cache original rotation speed;

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

        LeaperAttackState = new LeaperAttackState(this);
        // LeaperHopState = new LeaperHopState(this);
        // LeaperChaseState = new LeaperChaseState(this);
        LeaperPatrolState = new LeaperPatrolState(this);


    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(HitBoxLocation.transform.position, HitBoxRadius);
    }
    public void CheckForHits()
    {
        Collider[] hitColliders = Physics.OverlapSphere(HitBoxLocation.transform.position, HitBoxRadius,HitLayer);

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



    // For the leaper's weapon hitbox when attack state is done
    public void FinishAnimation() 
    {
        IsAttackAnimationPlaying = false;
        //DisableWeaponTriggers();
    }


    // From this returned Tween object you can attach things like OnComplete() and OnUpdate() for more control over tween.
    public Tween TweenLeap(Vector3 leapDestination, float leapDuration, float leapHeight)
    {
        Vector3 startPoint = RigidBody.position;
        Vector3 endPoint = leapDestination;
        Vector3 midPoint = (startPoint + endPoint) / 2;
        midPoint.y += leapHeight;
        Vector3[] path = { startPoint, midPoint, endPoint };
        return RigidBody.DOPath(path, leapDuration, PathType.CatmullRom).SetEase(Ease.Linear);
    }

   





}
