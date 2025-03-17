using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{
    public static ObjectPoolerManager Instance { get; private set; }    

    [Header("Settings")]
    [SerializeField] private int defaultCapacity = 100;
    [SerializeField] private int defaultMaxSize = 150;

    [field: Header("Commonly Used Prefabs")]
    [field: SerializeField] public HitNumbers HitNumbersPrefab { get; private set; }
    [field: SerializeField] public MaterializeVFX MaterializeVFXPrefab { get; private set; }

    /// <summary>
    /// A dictionary that stores all object poolers as values and their prefab by key.
    /// </summary>
    private Dictionary<GameObject, ObjectPooler> objectPoolerDictionary = new();

    private void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;
    }

    /// <summary>
    /// Spawns a prefab by using an existing pooler or creating a new one if it doesnt exist.
    /// </summary>
    /// <typeparam name="T">The type of the newPrefab.</typeparam>
    /// <param name="newPrefab">The prefrab you want to spawn</param>
    /// <param name="position">The position you want to spawn the prefab at</param>
    /// <param name="parent">The parent of the new spawned object</param>
    /// <returns></returns>
    public T SpawnPooledObject<T>(GameObject newPrefab, Vector3? position = null, Transform parent = null) where T : Component
    {
        if (objectPoolerDictionary.ContainsKey(newPrefab))
        {
            ObjectPooler pooler = objectPoolerDictionary[newPrefab];
            return pooler.SpawnObject<T>(position, parent);
        }

        GameObject newPoolerGameObject = new GameObject($"{newPrefab.name}Pooler");
        newPoolerGameObject.transform.SetParent(transform);
        ObjectPooler newPooler = newPoolerGameObject.AddComponent<ObjectPooler>();
        newPooler.Init(newPrefab, defaultCapacity, defaultMaxSize);
        objectPoolerDictionary.Add(newPrefab, newPooler);

        return newPooler.SpawnObject<T>(position, parent);
    }
}
