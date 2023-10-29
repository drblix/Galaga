using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Galaga
{
    public class ObjectPool : MonoBehaviour
    {
        public enum PooledObject
        {
            Fighter,
            Soldier,
            Knight,
            Boss,
            PlayerMissile,
            EnemyMissile
        }

        [SerializeField, Tooltip("Objects to include in the object pool")]
        private GameObject[] _objsToPool;
        [SerializeField, Tooltip("Number of objects to initially pool"), Range(0, 100)]
        private uint[] _poolCount;

        private void Awake()
        {
            
        }
    }
}
