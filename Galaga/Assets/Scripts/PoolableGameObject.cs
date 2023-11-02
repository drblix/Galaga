using UnityEngine;

namespace Galaga
{
    public class PoolableGameObject : MonoBehaviour
    {
        public PooledObject ObjectType
        {
            get { return _pooledObject; }
        }

        [SerializeField]
        private PooledObject _pooledObject;

        public void PoolObject() => ObjectPool.PoolObject(this);

        public virtual void OnSpawn() { }
    }
}

