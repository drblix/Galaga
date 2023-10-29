using System;
using System.Collections.Generic;
using UnityEngine;

namespace Galaga
{
    public class ObjectPool : MonoBehaviour
    {
        private static ObjectPool _singleton;

        // array of object pools for each specified prefab in the "_objsToPool"
        // PooledObject enum corresponds to each list of pooled objects; example: Soldier = 1, so objectPool[1] will contain the list of pooled soldier objects
        private static List<PoolableGameObject>[] objectPool;


        [SerializeField, Tooltip("Objects to include in the object pool")]
        private PoolableGameObject[] _objsToPool;
        [SerializeField, Tooltip("Number of objects to initially pool"), Range(0, 100)]
        private uint[] _poolCount;


        private void Awake()
        {
            _singleton = this;

            if (_objsToPool.Length != _poolCount.Length) throw new Exception($"{nameof(_objsToPool)} array length does not match {nameof(_poolCount)} array");

            // initializing object pool
            objectPool = new List<PoolableGameObject>[_objsToPool.Length];

            // filling object pool array with lists of pooled objects
            for (uint i = 0; i < _objsToPool.Length; i++)
            {
                if (_objsToPool[i] == null)
                {
                    Debug.LogError("Object in pool array is null; is the object poolable?");
                    continue;
                }

                List<PoolableGameObject> tempPool = new();
                for (uint j = 0; j < _poolCount[i]; j++)
                {
                    GameObject newObj = Instantiate(_objsToPool[i].gameObject, transform);
                    tempPool.Add(newObj.GetComponent<PoolableGameObject>());
                }

                objectPool[i] = tempPool;
            }
        }

        public static GameObject GetObject(PooledObject type)
        {
            uint index = (uint)type;
            PoolableGameObject obj = objectPool[index].Pop();

            if (obj == null)
            {
                Debug.LogWarning($"There's not enough objects of type {type} in the object pool. Creating new object!");
                GameObject newObj = Instantiate(_singleton._objsToPool[index].gameObject, _singleton.transform);
                return newObj;
            }

            return obj.gameObject;
        }

        public static GameObject GetObject(PooledObject type, SpawnParameters parameters)
        {
            GameObject newObj = GetObject(type);

            newObj.SetActive(parameters.SpawnActive);
            newObj.transform.SetPositionAndRotation(parameters.SpawnPosition, parameters.SpawnRotation);
            newObj.transform.SetParent(parameters.SpawnParent);

            return newObj;
        }

        public static void PoolObject(PoolableGameObject poolableObject)
        {
            uint index = (uint)poolableObject.ObjectType;
            objectPool[index].Add(poolableObject);
            poolableObject.gameObject.SetActive(false);
            poolableObject.transform.SetParent(_singleton.transform);
        }
    }

    public enum PooledObject
    {
        Fighter,
        Soldier,
        Knight,
        Boss,
        PlayerMissile,
        EnemyMissile
    }

    public readonly struct SpawnParameters
    {
        public readonly bool SpawnActive
        {
            get { return _spawnActive; }
        }

        public readonly Transform SpawnParent
        {
            get { return _spawnParent; }
        }

        public readonly Vector3 SpawnPosition
        {
            get { return _spawnPosition; }
        }

        public readonly Quaternion SpawnRotation
        {
            get { return _spawnRotation; }
        }

        private readonly bool _spawnActive;
        private readonly Transform _spawnParent;
        private readonly Vector3 _spawnPosition;
        private readonly Quaternion _spawnRotation;

        public SpawnParameters(Vector3 spawnPos, Quaternion spawnRotation, Transform parent, bool active)
        {
            this._spawnActive = active;
            this._spawnParent = parent;
            this._spawnPosition = spawnPos;
            this._spawnRotation = spawnRotation;
        }

        public SpawnParameters(Vector3 spawnPos, Quaternion spawnRotation, Transform parent)
        {
            this._spawnActive = true;
            this._spawnParent = parent;
            this._spawnPosition = spawnPos;
            this._spawnRotation = spawnRotation;
        }

        public SpawnParameters(Vector3 spawnPos, Quaternion spawnRotation)
        {
            this._spawnActive = true;
            this._spawnParent = null;
            this._spawnPosition = spawnPos;
            this._spawnRotation = spawnRotation;
        }

        public SpawnParameters(Vector3 spawnPos)
        {
            this._spawnActive = true;
            this._spawnParent = null;
            this._spawnRotation = Quaternion.identity;
            this._spawnPosition = spawnPos;
        }
    }
}
