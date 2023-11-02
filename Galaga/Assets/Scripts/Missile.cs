using UnityEngine;

namespace Galaga
{
    public class Missile : PoolableGameObject
    {
        [SerializeField]
        private bool _player = true;

        [SerializeField]
        private float _speed = 3f;

        [SerializeField]
        private Rigidbody2D _rigidbody;

        private Vector2 _start = Vector2.zero, _end = Vector2.zero;

        public void Initialize()
        {
            _start = transform.position;
            if (_player)
                _end = new Vector2(_start.x, _start.y + 20f);
            else
                _end = FindAnyObjectByType<Fighter>().transform.position;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, _end - _start);

            _rigidbody.velocity = (_end - _start) * _speed;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider != null && collision.collider.TryGetComponent(out IDestroyable destroyable))
                destroyable.Hit();

            PoolObject();
        }
    }
}
