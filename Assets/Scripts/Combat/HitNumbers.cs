using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Pool;
using DG.Tweening;
using DreamscapeObjectPooler;

public class HitNumbers : MonoBehaviour, IPoolableObject
{
    [Header("Settings")]
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private float minTextSize = 200f;
    [SerializeField] private float maxTextSize = 400f;
    [SerializeField] private int maxDamage = 10000;

    private ObjectPool<GameObject> pool;

    private void OnDisable()
    {
        DOTween.Kill(transform);
        DOTween.Kill(numberText);
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void ActivateHitNumberText(int damage, Vector3 spawnPoint, Vector3 direction, Color color)
    {
        transform.position = spawnPoint;

        numberText.text = damage.ToString();
        numberText.fontSize = (maxTextSize - minTextSize) * (damage / (float)maxDamage) + minTextSize;
        numberText.color = color;

        FloatAndFade(2f, 1f, direction);
    }

    private void FloatAndFade(float duration, float distance, Vector3 direction)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(transform.position + distance * direction, duration / 2f).SetEase(Ease.OutCubic).SetUpdate(true));
        sequence.Append(numberText.DOFade(0f, duration / 2f)).SetUpdate(true);

        sequence.OnComplete(() => { pool.Release(gameObject); });
    }

    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
}
