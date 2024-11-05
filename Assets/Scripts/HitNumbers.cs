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

    private void LateUpdate()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void ActivateHitNumberText(int damage, Vector3 spawnPoint)
    {
        transform.position = spawnPoint;

        numberText.text = damage.ToString();
        numberText.fontSize = (maxTextSize - minTextSize) * (damage / (float)maxDamage) + minTextSize;

        FloatUpAndFade(2f, 1f);
    }

    private void FloatUpAndFade(float duration, float distance)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(numberText.DOColor(Color.red, duration / 2f)).SetUpdate(true).OnComplete(() => {
            transform.DOMoveY(transform.position.y + distance, duration / 2f).SetUpdate(true);
        });
        sequence.Append(numberText.DOFade(0f, duration / 2f)).SetUpdate(true);

        sequence.OnComplete(() => { pool.Release(gameObject); });
    }

    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
}
