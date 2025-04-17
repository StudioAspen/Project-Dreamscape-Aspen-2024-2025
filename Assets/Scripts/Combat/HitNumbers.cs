using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Pool;
using DG.Tweening;

public class HitNumbers : MonoBehaviour, IPoolableObject
{
    [Header("Settings")]
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private float minTextSize = 200f;
    [SerializeField] private float maxTextSize = 400f;
    [SerializeField] private int maxDamage = 10000;

    private ObjectPool<GameObject> pool;

    private float startScale;

    private void Awake()
    {
        startScale = transform.localScale.x;
    }

    private void OnDisable()
    {
        // cancel all tweens
        DOTween.Kill(transform);
        DOTween.Kill(numberText);
    }

    private void LateUpdate()
    {
        // look at the main camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    /// <summary>
    /// Activates the hit number text with the specified damage, spawn point, direction, and color.
    /// Needs to be called when spawned.
    /// </summary>
    /// <param name="damage">The damage value.</param>
    /// <param name="spawnPoint">The spawn point of the hit number text.</param>
    /// <param name="direction">The direction in which the hit number text should float.</param>
    /// <param name="color">The color of the hit number text.</param>
    public void ActivateHitNumberText(int damage, Vector3 spawnPoint, Vector3 direction, Color color)
    {
        transform.position = spawnPoint;
        transform.localScale = Vector3.zero;

        numberText.text = damage.ToString();
        numberText.fontSize = Mathf.Lerp(minTextSize, maxTextSize, damage / (float)maxDamage);
        numberText.color = color;

        FloatAndFade(2f, 1f, direction);
    }

    /// <summary>
    /// Floats and fades the hit number text for a specified duration, distance, and direction.
    /// </summary>
    /// <param name="duration">The duration of the floating and fading animation.</param>
    /// <param name="distance">The distance the hit number text should float.</param>
    /// <param name="direction">The direction in which the hit number text should float.</param>
    private void FloatAndFade(float duration, float distance, Vector3 direction)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(transform.position + distance * direction, duration / 2f).SetEase(Ease.OutCubic).SetUpdate(true));
        sequence.Append(numberText.DOFade(0f, duration / 2f)).SetUpdate(true);

        transform.DOScale(startScale * Vector3.one, duration / 5f).SetEase(Ease.OutCubic).SetUpdate(true);

        sequence.OnComplete(() => { pool.Release(gameObject); });
    }

    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
}
