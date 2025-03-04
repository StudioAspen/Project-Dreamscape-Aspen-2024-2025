using UnityEngine;
using UnityEngine.Pool;

namespace Dreamscape.Abilities
{
    public abstract class CastedAbility : MonoBehaviour, IPoolableObject
    {
        private protected Entity casterEntity;

        private protected ObjectPool<GameObject> pool;

        /// <summary>
        /// Initialized the ability with the entity caster.
        /// </summary>
        /// <param name="caster">The entity caster.</param>
        public void Init(Entity caster)
        {
            casterEntity = caster;

            OnSpawn();
        }

        /// <summary>
        /// Logic for when the ability is spawned
        /// </summary>
        private protected abstract void OnSpawn();

        private void OnDisable()
        {
            OnOnDisable();
        }

        /// <summary>
        /// Logic for when the ability gets released to the pool
        /// </summary>
        private protected abstract void OnOnDisable();

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
}
