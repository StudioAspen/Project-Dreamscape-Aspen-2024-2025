using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EggYolkBaseState : ObstacleBaseState
{
    private protected EggYolk eggYolk;


    private protected override void Init()
    {
        eggYolk = obstacle as EggYolk;
    }
}

