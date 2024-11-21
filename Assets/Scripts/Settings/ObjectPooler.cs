using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace DreamscapeObjectPooler
{
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
            T component = spawnedObject.GetComponent<T>();

            Debug.Assert(component != null, $"Prefab is missing {component.GetType()} component");

            return component;
        }

        public T SpawnObject<T>(Vector3 position) where T : Component
        {
            GameObject spawnedObject = objectPool.Get();
            spawnedObject.transform.position = position;

            T component = spawnedObject.GetComponent<T>();

            Debug.Assert(component != null, $"Prefab is missing {component.GetType()} component");

            return component;
        }

        public T SpawnObject<T>(Vector3 position, Transform parent) where T : Component
        {
            GameObject spawnedObject = objectPool.Get();
            spawnedObject.transform.position = position;
            spawnedObject.transform.SetParent(parent);

            T component = spawnedObject.GetComponent<T>();

            Debug.Assert(component != null, $"Prefab is missing {component.GetType()} component");

            return component;
        }

        public void ReleaseObject(GameObject pooledObject)
        {
            objectPool.Release(pooledObject);
        }
        #endregion
    }

    public interface IPoolableObject
    {
        void SetObjectPool(ObjectPool<GameObject> objectPool);
    }
}

