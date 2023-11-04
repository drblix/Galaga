using Galaga.AI;
using UnityEngine;

namespace Galaga
{
    public class SpotManager : MonoBehaviour
    {
        private const float HALF_PI = Mathf.PI / 2f;

        private static SpotManager _singleton;

        [SerializeField]
        private Spot[] _spots;

        [SerializeField]
        private float _frequency = 12f;

        private float _lerpTimer = 0f;
        private float _counter = 0f;

        private void Awake()
        {
            _singleton = this;
        }

        private void Update()
        {
            foreach (Spot spot in _spots)
                spot.Lerp(_lerpTimer);

            _lerpTimer = Mathf.Abs(Mathf.Sin(Mathf.PI * _counter / _frequency));
            _counter = (_counter + Time.deltaTime) % LoopThreshold();
        }

        private float LoopThreshold() => HALF_PI * (_frequency / Mathf.PI) * 2f;

        public static Spot GetAvailableSpot(Enemy enemy)
        {
            PooledObject enemyType = enemy.ObjectType;

            foreach (Spot spot in _singleton._spots)
                if (!spot.IsFilled && spot.PreferredEnemy == enemyType) return spot;

            return null;
        }
    }
}
