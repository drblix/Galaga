using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Galaga
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Returns the first instance of an object in the list, removing it from the list in the process
        /// </summary>
        public static T Pop<T>(this List<T> list)
        {
            if (list.Count != 0)
            {
                int index = list.Count - 1;
                T obj = list[index];
                list.RemoveAt(index);

                return obj;
            }
            else
                return default;
        }

        /// <summary>
        /// Attempts to the pool the specified game object back into the object pool
        /// </summary>
        /// <param name="gameObject"></param>
        public static void PoolObject(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent<PoolableGameObject>(out PoolableGameObject poolableGameObject))
                poolableGameObject.PoolObject();
            else
                Debug.LogError($"{gameObject.name} is NOT a poolable game object!");
        }

        /// <summary>
        /// Returns the angle (in degrees) that looks from the start position to the end position
        /// </summary>
        public static float GetLookAngle(Vector2 start, Vector2 end) => Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
    }
}

