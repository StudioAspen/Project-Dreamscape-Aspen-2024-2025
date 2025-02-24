using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldVFX : MonoBehaviour
{
    private Material shieldMaterial;

    [Header("Config")]
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Animation")]
    [SerializeField] private float startAnimationDuration = 0.5f;
    [SerializeField] private float endAnimationDuration = 0.2f;
    [SerializeField] private Ease startAnimationEase;
    [SerializeField] private Ease endAnimationEase;
    [SerializeField, Range(0.01f, 10f)] private float startJitterScale = 0.01f;
    [SerializeField, Range(0f, 1f)] private float startOverallTransparency = 0f;
    [SerializeField, Range(0.01f, 10f)] private float endJitterScale = 2.76f;
    [SerializeField, Range(0f, 1f)] private float endOverallTransparency = 0.96f;
    private string jitterScaleProperty = "_JitterScale";
    private string overallTransparencyProperty = "_OverallTransparency";

    private Transform followTargetTransform;
    private Vector3 followOffset;
    private float rotationDirection;

    /// <summary>
    /// Initializes the shield visual effects with the specified size, follow target, and offset.
    /// </summary>
    /// <param name="size">The size of the shield.</param>
    /// <param name="followTarget">The transform to follow.</param>
    /// <param name="offset">The offset from the follow target.</param>
    public void Init(float size, Transform followTarget, Vector3 offset)
    {
        transform.localScale = size * Vector3.one;
        followTargetTransform = followTarget;
        followOffset = offset;
    }

    private void Awake()
    {
        shieldMaterial = GetComponent<Renderer>().material;
    }

    private void Start()
    {
        rotationDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1; // Randomly rotate the shield clockwise or counterclockwise
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);

        if (followTargetTransform != null) transform.position = followTargetTransform.position + followOffset;
    }

    /// <summary>
    /// Plays the start animation of the shield visual effects.
    /// </summary>
    /// <param name="onCompleteCallback">Optional callback to invoke when the animation completes.</param>
    public void PlayStartAnimation(Action onCompleteCallback = null)
    {
        shieldMaterial.SetFloat(jitterScaleProperty, startJitterScale);
        shieldMaterial.SetFloat(overallTransparencyProperty, startOverallTransparency);

        shieldMaterial.DOFloat(endJitterScale, jitterScaleProperty, startAnimationDuration).SetUpdate(false).SetEase(startAnimationEase);
        shieldMaterial.DOFloat(endOverallTransparency, overallTransparencyProperty, startAnimationDuration).SetUpdate(false).SetEase(startAnimationEase).OnComplete(() =>
        {
            onCompleteCallback?.Invoke();
        });
    }

    /// <summary>
    /// Plays the end animation of the shield visual effects.
    /// </summary>
    /// <param name="onCompleteCallback">Optional callback to invoke when the animation completes.</param>
    public void PlayEndAnimation(Action onCompleteCallback = null)
    {
        DOTween.Kill(shieldMaterial);
    
        shieldMaterial.DOFloat(startJitterScale, jitterScaleProperty, endAnimationDuration).SetUpdate(false).SetEase(endAnimationEase);
        shieldMaterial.DOFloat(startOverallTransparency, overallTransparencyProperty, endAnimationDuration).SetUpdate(false).SetEase(endAnimationEase).OnComplete(() => {
            onCompleteCallback?.Invoke();
        });
    }
}
