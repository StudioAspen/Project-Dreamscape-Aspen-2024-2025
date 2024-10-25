using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SelectionSphere : MonoBehaviour, IPoolableObject
{
    public bool CanBeSelected { get; private set; } = false;

    [SerializeField] private float spawnDuration = 1f;
    [field: SerializeField] public Vector2Int DesiredIslandSpawnPosition { get; private set; }
    private ObjectPool<GameObject> pool;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        CanBeSelected = false;
    }

    public void SetDesiredIslandSpawn(Vector2Int spawnPos)
    {
        DesiredIslandSpawnPosition = spawnPos;
        transform.DOMove(new Vector3(transform.position.x, 0f, transform.position.z), spawnDuration).SetEase(Ease.OutQuint).OnComplete(() => CanBeSelected = true);
    }

    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
}
