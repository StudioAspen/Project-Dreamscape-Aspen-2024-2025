using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linq;
using UnityEngine.VFX;
using System.Security.Principal;

public class EggYolk : Obstacle

{
    [field: Header("EggYolk: States")]
    [field: SerializeField] public EggYolkIdleState EggYolkIdleState { get; private set; }
    [field: SerializeField] public EggYolkOnHitState EggYolkOnHitState { get; private set; }
    [field: SerializeField] public EggYolkExplosionState EggYolkExplosionState { get; private set; }
    [field: SerializeField] public EggYolkCooldownState EggYolkCooldownState { get; private set; }

    pritvate protected override void OnAwake()
    {

    }
    pritvate protected override void OnState()
    {

    }

    pritvate protected override void OnUpdate()
    {

    }

}
