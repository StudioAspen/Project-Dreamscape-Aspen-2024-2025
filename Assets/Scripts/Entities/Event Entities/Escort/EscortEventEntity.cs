using UnityEngine;

public class EscortEventEntity : Enemy
{
    [field: Header("Escort Event Entity: Wander Settings")]
    [field: SerializeField] public Vector2 WanderIntervalDurationRange { get; private set; } = new Vector2(7f, 10f);
    [field: SerializeField] public Vector2 WanderRadiusRange { get; private set; } = new Vector2(7f, 10f);

    #region States
    public EscortEventEntityWanderStateSO EscortEventEntityWanderState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        EscortEventEntityWanderState = EntityBaseStateSO.CreateRuntimeInstance<EscortEventEntityWanderStateSO>(this);
    }
    #endregion

    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(EscortEventEntityWanderState);
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        ChangeTeam(0);

        SetDefaultState(EscortEventEntityWanderState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }
}