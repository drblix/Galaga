using UnityEngine;

namespace Galaga
{
    public class Fighter : PoolableGameObject, IDestroyable
    {
        [SerializeField, Tooltip("Position where player shots are created relative to player's position")]
        private Vector2 _shootPos;

        [SerializeField, Tooltip("Speed the player moves")]
        private float _moveSpeed = 5f;

        [SerializeField, Tooltip("Side-to-side positional X restriction")]
        private float _xBorder = 8.3f;


        private Rigidbody2D _rigidbody;

        public void Hit()
        {
            Debug.Log("player hit! (should die instantly)");
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            PlayerInputEvents.OnPlayerMove += OnMove;
            PlayerInputEvents.OnPlayerShoot += OnShoot;
        }

        private void Update()
        {
            float newX = Mathf.Clamp(transform.position.x, -_xBorder, _xBorder);
            transform.position = new Vector2(newX, transform.position.y);
        }

        private void OnShoot()
        {
            GameObject newMissile = ObjectPool.GetObject(PooledObject.PlayerMissile, new SpawnParameters((Vector2)transform.position + _shootPos, Quaternion.identity, null, true));
            newMissile.GetComponent<Missile>().Initialize();
        }

        private void OnMove(float x) => _rigidbody.velocity = x * _moveSpeed * Vector2.right;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + _shootPos, Vector3.one * .05f);
        }
    }
}
