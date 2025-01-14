using UnityEngine;

public class EscortEventEntity : Enemy
{
    #region States
    public EscortEventEntityWanderState EscortEventEntityWanderState { get; private set; }

    private protected override void InitializeStates()
    {
        base.InitializeStates();

        EscortEventEntityWanderState = EntityBaseState.InitializeOrCreate<EscortEventEntityWanderState>(this);
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