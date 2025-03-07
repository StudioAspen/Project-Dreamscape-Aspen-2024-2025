using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class ObjectPooler : MonoBehaviour
{
    private GameObject objectPrefab;

    private ObjectPool<GameObject> objectPool;

    public void Init(GameObject objectPrefab, int capacity, int maxSize)
    {
        this.objectPrefab = objectPrefab;
        objectPool = new ObjectPool<GameObject>(CreateObject, OnGetFromPool, OnReleaseToPool, OnDestroyObject, true, capacity, maxSize);
    }

    private GameObject CreateObject()
    {
        GameObject o = Instantiate(objectPrefab, new Vector3(0f, 100000f, 0f), Quaternion.identity, transform);
        o.GetComponent<IPoolableObject>().SetObjectPool(objectPool);

        return o;
    }

    private void OnGetFromPool(GameObject pooledObject)
    {
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
    public T SpawnObject<T>(Vector3? position = null, Transform parent = null) where T : Component
    {
        GameObject spawnedObject = objectPool.Get();

        // Set position if provided, otherwise default to zero
        spawnedObject.transform.position = position ?? Vector3.zero;

        // Set parent if provided
        if (parent != null)
        {
            spawnedObject.transform.SetParent(parent);
        }

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
