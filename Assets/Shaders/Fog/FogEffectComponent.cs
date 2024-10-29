using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/FogEffect", typeof(UniversalRenderPipeline))]

public class FogEffectComponent : VolumeComponent, IPostProcessComponent
{

    public ColorParameter primaryFogColor = new ColorParameter(Color.white, true, true, true);
    public MinFloatParameter fogDensity = new MinFloatParameter(0.0f, 0.0f);
    public FloatParameter fogOffset = new FloatParameter(1f);

    public bool IsActive() => fogDensity != 0;
    public bool IsTileCompatible() => false;
}