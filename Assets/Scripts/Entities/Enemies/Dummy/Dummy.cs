using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    private protected override void OnAwake()
    {
        base.OnAwake();
    }

    private protected override void OnOnEnable()
    {
        base.OnOnEnable();

        SetStartState(EntityEmptyState);
    }

    private protected override void OnOnDisable()
    {
        base.OnOnDisable();
    }

    private protected override void OnStart()
    {
        base.OnStart();

        SetDefaultState(EntityEmptyState);
    }

    private protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    private protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    private protected override void OnOnDrawGizmos()
    {
        base.OnOnDrawGizmos();
    }

    // Override this method to make the dummy not take damage
    public override void TakeDamage(int damage, Vector3 hitPoint, GameObject source, bool willTryStagger = true, bool willIgnoreDefense = false)
    {
        if (CurrentState == EntityDeathState) return;

        if (willTryStagger) TryChangeStaggeredState();

        AttemptToSpawnHitNumbers(damage, hitPoint, Color.red);

        OnEntityTakeDamage?.Invoke(damage, hitPoint, source);

        lastHitSource = source;
    }
}
