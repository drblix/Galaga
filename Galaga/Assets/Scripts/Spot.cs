using UnityEngine;
using Galaga.AI;
using TreeEditor;

namespace Galaga
{
    public class Spot : MonoBehaviour
    {
        public Enemy FilledEnemy
        {
            get { return _enemy; }
            set { _enemy = value; }
        }

        public bool IsFilled
        {
            get { return _enemy != null; }
        }

        public PooledObject PreferredEnemy
        {
            get { return _preferredEnemy; }
        }

        [SerializeField]
        private Enemy _enemy;

        [SerializeField]
        private PooledObject _preferredEnemy;

        [SerializeField]
        private Vector2 _endOffset = Vector2.one * .5f;

        private Vector2 _start;
        private Vector2 _end;

        private void Start()
        {
            _start = transform.position;
            _end = _start + _endOffset;
        }

        public void Lerp(float t) => transform.position = Vector2.Lerp(_start, _end, t);

        private void OnDrawGizmos()
        {
            Gizmos.color = _preferredEnemy switch
            {
                PooledObject.Soldier => Color.red,
                PooledObject.Knight => Color.yellow,
                PooledObject.Boss => Color.blue,
                _ => Color.white,
            };

            Vector2 lerpGoal = (Vector2)transform.position + _endOffset;
            Gizmos.DrawWireCube(transform.position, Vector3.one * .35f);
            Gizmos.DrawWireCube(lerpGoal, Vector3.one * .15f);
            Gizmos.DrawLine(transform.position, lerpGoal);
        }
    }
}

