using UnityEngine;
using UnityEngine.Pool;

public class CastedAbility : MonoBehaviour, IPoolableObject
{
    private protected GameObject caster;
    private protected int team;

    private ObjectPool<GameObject> pool;

    public void Init(GameObject caster, int team)
    {
        this.caster = caster;
        this.team = team;
    }

    private void OnEnable()
    {
        caster = null;
        team = 0;

        OnOnEnable();
    }

    private protected virtual void OnOnEnable()
    {

    }

    private void OnDisable()
    {
        // logic for when the fireball gets released to the pool
        OnOnDisable();
    }

    private protected virtual void OnOnDisable()
    {

    }

    /// <summary>
    /// Attempts to release the object back to the pool. If the pool doesn't exist, then Destroy
    /// </summary>
    public void DestroyAndRelease()
    {
        if (pool == null)
        {
            Destroy(gameObject);
            return;
        }

        pool.Release(gameObject);
    }

    public void SetObjectPool(ObjectPool<GameObject> objectPool)
    {
        pool = objectPool;
    }
}
