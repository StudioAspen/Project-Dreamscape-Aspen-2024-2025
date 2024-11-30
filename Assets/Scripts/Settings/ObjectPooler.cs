using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class ObjectPooler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject objectPrefab;

    private ObjectPool<GameObject> objectPool;

    [Header("Settings")]
    [SerializeField] private int defaultCapacity = 100;
    [SerializeField] private int maxSize = 150;

    private void Awake()
    {
        objectPool = new ObjectPool<GameObject>(CreateObject, OnGetFromPool, OnReleaseToPool, OnDestroyObject, true, defaultCapacity, maxSize);
    }

    public void ChangePrefab(GameObject prefab)
    {
        objectPrefab = prefab;
    }

    private GameObject CreateObject()
    {
        GameObject o = Instantiate(objectPrefab, transform);
        o.GetComponent<IPoolableObject>().SetObjectPool(objectPool);

        return o;
    }

    private void OnGetFromPool(GameObject pooledObject)
    {
        pooledObject.transform.position = new Vector3(0f, 100000f, 0f);
        pooledObject.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(GameObject pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }

    private void OnDestroyObject(GameObject pooledObject)
    {
        Destroy(pooledObject);
    }

    #region Factory
    public T SpawnObject<T>() where T : Component
    {
        GameObject spawnedObject = objectPool.Get();
        spawnedObject.transform.position = Vector3.zero;
        Physics.SyncTransforms();

        T component = spawnedObject.GetComponent<T>();
        
        Debug.Assert(component != null, $"Prefab is missing {typeof(T)} component");

        return component;
    }

    public T SpawnObject<T>(Vector3 position) where T : Component
    {
        GameObject spawnedObject = objectPool.Get();
        spawnedObject.transform.position = position;
        Physics.SyncTransforms();

        T component = spawnedObject.GetComponent<T>();

        Debug.Assert(component != null, $"Prefab is missing {typeof(T)} component");

        return component;
    }

    public T SpawnObject<T>(Vector3 position, Transform parent) where T : Component
    {
        GameObject spawnedObject = objectPool.Get();
        spawnedObject.transform.position = position;
        spawnedObject.transform.SetParent(parent);
        Physics.SyncTransforms();

        T component = spawnedObject.GetComponent<T>();

        Debug.Assert(component != null, $"Prefab is missing {typeof(T)} component");

        return component;
    }

    public void ReleaseObject(GameObject pooledObject)
    {
        objectPool.Release(pooledObject);
    }
    #endregion
}

/// <summary>
/// Represents an object that can be pooled in an object pool.
/// Interface this to allow pooled objects to release themselves back to the pool.
/// </summary>
public interface IPoolableObject
{
    /// <summary>
    /// Sets the object pool that manages this pooled object.
    /// Must be used for poolable objects to be able to release themselves back to the pool.
    /// Must assign your local object pool reference to the object pool parameter.
    /// </summary>
    /// <param name="objectPool">The object pool to set.</param>
    void SetObjectPool(ObjectPool<GameObject> objectPool);
}
