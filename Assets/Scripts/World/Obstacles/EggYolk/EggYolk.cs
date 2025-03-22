using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
using System.Security.Principal;

public class EggYolk : Obstacle

{
    [field: Header("EggYolk: States")]
    [field: SerializeField] public EggYolkIdleState EggYolkIdleState { get; private set; }
    [field: SerializeField] public EggYolkOnHitState EggYolkOnHitState { get; private set; }
    [field: SerializeField] public EggYolkExplosionState EggYolkExplosionState { get; private set; }
    [field: SerializeField] public EggYolkCooldownState EggYolkCooldownState { get; private set; }

    private protected override void OnAwake()
    {

    }
    private protected override void OnStart()
    {

    }

    private protected override void OnUpdate()
    {

    }

}
