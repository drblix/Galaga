using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaga.AI;

namespace Galaga
{
    public class RoundManager : MonoBehaviour
    {
        public static RoundManager Singleton
        {
            get { return _singleton; }
        }

        private static RoundManager _singleton;

        public List<Enemy> SpawnedEnemies
        {
            get { return _spawnedEnemies; }
        }

        private readonly List<Enemy> _spawnedEnemies = new();

        private void Awake()
        {
            _singleton = this;
        }

        public bool CanDive()
        {
            ushort diving = 0;
            foreach (Enemy enemy in _spawnedEnemies)
            {
                if (enemy.State == EnemyState.Diving)
                    diving++;

                if (diving >= 2)
                    return false;
            }

            return true;
        }
    }
}

