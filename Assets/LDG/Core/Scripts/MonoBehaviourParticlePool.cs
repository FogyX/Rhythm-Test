// Sound Reactor
// Copyright (c) 2018, Little Dreamer Games, All Rights Reserved
// Please visit us at littledreamergames.com

using UnityEngine;

namespace LDG.Core
{
    using LDG.Core.Collections.Generic;

    public class MonoBehaviourParticlePool : MonoBehaviour, IParticleCollection
    {
        public MonoBehaviourParticle Particle;
        public int PoolSize = 1;

        private ObjectPool<GameObject> pool;

        // Start is called before the first frame update
        void Start()
        {
            // disable this game object if there isn't a particle to cache
            if (!Particle)
            {
                gameObject.SetActive(false);

                Debug.LogError("Particle has not been assigned. Disabling Pool.", gameObject);

                return;
            }

            if (PoolSize < 1)
            {
                gameObject.SetActive(false);

                Debug.LogError("PoolSize cannot be less than 1. Disabling Pool.", gameObject);

                return;
            }

            InitPool(PoolSize);
        }

        public MonoBehaviourParticle GetParticle(bool enableGameObject = true)
        {
            if (pool == null || pool.Length == 0)
            {
                Debug.LogError("Pool is not initialized", gameObject);
                return null;
            }

            MonoBehaviourParticle poolObject = pool.Get().GetComponent<MonoBehaviourParticle>();
            pool++;

            poolObject.gameObject.SetActive(enableGameObject);

            return poolObject;
        }

        public void ReturnParticle(MonoBehaviourParticle particle)
        {
            if (pool == null)
            {
                Debug.LogError("Pool is not initialized", gameObject);
            }

            particle.gameObject.transform.SetParent(transform);
            particle.gameObject.SetActive(false);
        }

        public void InitPool(int size)
        {
            MonoBehaviourParticle particle;
            PoolSize = size;
            pool = new ObjectPool<GameObject>(size);

            for (int i = 0; i < size; i++)
            {
                GameObject go = Instantiate(this.Particle.gameObject);
                go.transform.SetParent(this.transform);
                go.gameObject.SetActive(false);

                particle = go.GetComponent<MonoBehaviourParticle>();
                particle.SetOwner(this);

                pool.Set(go, i);
            }
        }

        public void DeactivateAll()
        {
            foreach(GameObject go in pool)
            {
                go.SetActive(false);
            }
        }
    }
}