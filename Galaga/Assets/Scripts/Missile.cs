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
                _end = Vector2.one;

            float lookAngle = Mathf.Atan2(_end.x - _start.x, _end.y - _start.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);

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
