using UnityEngine;

public class Charger : Enemy
{
    [field: Header("Follower: Attack Settings")]
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;
    [field: SerializeField] public Vector2Int AttackDamageRange { get; private set; } = new Vector2Int(10, 15);
    

    [field: Header("Charger: Idle Settings")]
   
    [field: SerializeField] public float IdleWanderWaitMin { get; private set; } = 1f;
    [field: SerializeField] public float IdleWanderWaitMax { get; private set; } = 3f;


    [field: Header("Charger: Wander Settings")]
    [field: SerializeField] public float WanderMaxRadius { get; private set; } = 5f;
    [field: SerializeField] public float WanderMinRadius { get; private set; } = 1.5f;


   
    [field: Header("Charger: Advanced Wander Settings")]
    [field: SerializeField] public float WanderRaycastDownMaxDistance = 15f;
    [field: SerializeField] public float WanderEndDistanceThreshold = 1.25f;
    [field: SerializeField] public float WanderTimeout = 10f;
    [field: SerializeField] public float WanderNavMeshSampleRadius = .1f;


    [field: Header("Charger: Dazed Settings")]
    [field: SerializeField] public float DazedDuration { get; private set; } = 5f;

    #region States
    public ChargerIdleState ChargerIdleState { get; private set; }
    public ChargerDazedState ChargerDazedState { get; private set; }
    public ChargerWanderState ChargerWanderState { get; private set; }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        ChargerIdleState = new ChargerIdleState(this);
        ChargerDazedState = new ChargerDazedState(this);
        ChargerWanderState = new ChargerWanderState(this);
    }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnFixedUpdate()
    {                 
        base.OnFixedUpdate();
    }

    protected override void OnOnAnimatorMove()
    {
        base.OnOnAnimatorMove();
    }

    protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    protected override void OnOnEnable()
    {
        //base.OnOnEnable();
         SetStartState(ChargerIdleState);
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }
}
