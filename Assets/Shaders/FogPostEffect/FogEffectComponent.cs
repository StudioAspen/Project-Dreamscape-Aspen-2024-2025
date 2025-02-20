using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenuForRenderPipeline("Custom/FogEffect", typeof(UniversalRenderPipeline))]
public class FogEffectComponent : VolumeComponent, IPostProcessComponent
{
    private const float TwoPi = 2 * Mathf.PI;

    [Header("Main Fog Parameters")]
    public ColorParameter primaryFogColor = new ColorParameter(Color.grey, true, true, true);
    public ColorParameter secondaryFogColor = new ColorParameter(Color.gray, true, true, true);
    public MinFloatParameter gradientStrength = new MinFloatParameter(0.0f, 0.0f);
    public FloatParameter fogOffset = new FloatParameter(5.0f);
    public FloatParameter primaryFogColorOffset = new FloatParameter(10.0f);
    public FloatParameter secondaryFogColorOffset = new FloatParameter(10.0f);
    public MinFloatParameter fogDensity = new MinFloatParameter(0.0f, 0.0f);
    public FloatParameter fogScattering = new FloatParameter(0.0f);

    [Header("Sky Box Parameters")]
    public ColorParameter skyBoxFogColor = new ColorParameter(Color.gray, true, true, true);
    public MinFloatParameter skyBoxFogDensity = new MinFloatParameter(0.0f, 0.0f);

    [Header("Noise Texture")]
    public TextureParameter noiseTexture = new TextureParameter(null);

    [Header("Fog Noise Parameters")]
    public ClampedFloatParameter rotateFogNoise = new ClampedFloatParameter(0.0f, 0.0f, TwoPi);
    public FloatParameter fogNoiseScale = new FloatParameter(0.0f);
    public FloatParameter fogNoiseVelocity = new FloatParameter(1.0f);

    [Header("Sky Box Noise Parameters")]
    public ClampedFloatParameter rotateSkyBoxNoise = new ClampedFloatParameter(0.0f, 0.0f, TwoPi);
    public FloatParameter skyBoxNoiseScale = new FloatParameter(1.0f);
    public FloatParameter skyBoxNoiseVelocity = new FloatParameter(1.0f);
    public FloatParameter skyBoxNoiseTransparency = new FloatParameter(0.0f);

    public bool IsActive() => fogDensity != 0;
    public bool IsTileCompatible() => false;
}