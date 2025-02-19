using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.VFX;

public class DreamBush : Obstacle
{
    [field: Header("DreamBush: States")]
    [field: SerializeField] public DreamBushNeutralState DreamBushNeutralState { get; private set; }
    [field: SerializeField] public DreamBushFriendlyState DreamBushFriendlyState { get; private set; }
    [field: SerializeField] public DreamBushHostileState DreamBushHostileState { get; private set; }

    [field: Header("DreamBush: Config")]
    [field: SerializeField] public Renderer Renderer { get; private set; }

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
