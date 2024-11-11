using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDazedState : EnemyBaseState
{

    private Coroutine dazedCoroutine;
    private float healthWhenEnterDazed;
    private Charger charger;

    public ChargerDazedState(Charger enemy) : base(enemy)
    {
        charger = enemy;
    }
    public override void OnEnter()
    {
        charger.SetSpeedModifier(0f);
        dazedCoroutine = charger.StartCoroutine(DazedCoroutine());
        healthWhenEnterDazed = charger.CurrentHealth;
        //Debug.Log("Enter charger dazed coroutine");
    }

    public override void OnExit()
    {
        if (dazedCoroutine != null)
        {
            charger.StopCoroutine(dazedCoroutine);
            dazedCoroutine = null;
        }
    }

    public override void Update()
    {
        if (charger.CurrentHealth < healthWhenEnterDazed)
        {
            charger.ChangeState(charger.ChargerDamagedState);
        }
    }

    public override void FixedUpdate() { }

    private IEnumerator DazedCoroutine()
    {
        yield return new WaitForSeconds(charger.DazedDuration);
        //Debug.Log("Charger dazed timer is over!");
        enemy.ChangeState(charger.ChargerIdleState);
    }

}