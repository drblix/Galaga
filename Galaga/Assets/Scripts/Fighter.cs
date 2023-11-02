using System.Collections;
using UnityEngine;
using Galaga.AI;
using Unity.VisualScripting;

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

        [SerializeField, Tooltip("How many seconds must pass before the player can shoot again")]
        private float _missileThreshold = .2f;

        private float _missileTimer = 0f;

        private Rigidbody2D _rigidbody;
        private AudioSource _mainSource;

        public void Hit()
        {
            Debug.Log("player hit! (should die instantly)");
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _mainSource = GetComponentInChildren<AudioSource>();

            PlayerInputEvents.OnPlayerMove += OnMove;
            PlayerInputEvents.OnPlayerShoot += OnShoot;

            StartCoroutine(TestRoutine());
        }

        private IEnumerator TestRoutine()
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject leftEnemy = ObjectPool.GetObject(PooledObject.Soldier, new ObjectPool.SpawnParameters(Vector3.zero, Quaternion.identity, null, true));
                leftEnemy.GetComponent<Enemy>().SpawnSequence(PathContainer.Singleton.GetCurve(CurveName.CloseGrazeFromLeft), 1.5f, 1f);

                GameObject rightEnemy = ObjectPool.GetObject(PooledObject.Soldier, new ObjectPool.SpawnParameters(Vector3.zero, Quaternion.identity, null, true));
                rightEnemy.GetComponent<Enemy>().SpawnSequence(PathContainer.Singleton.GetCurve(CurveName.CloseGrazeFromRight), 1.5f, 1f);

                yield return new WaitForSeconds(.25f);
            }
        }

        private void Update()
        {
            float newX = Mathf.Clamp(transform.position.x, -_xBorder, _xBorder);
            transform.position = new Vector2(newX, transform.position.y);

            if (_missileTimer < _missileThreshold)
                _missileTimer += Time.deltaTime;
        }

        private void OnShoot()
        {
            if (_missileTimer < _missileThreshold) return;

            GameObject newMissile = ObjectPool.GetObject(PooledObject.PlayerMissile, new ObjectPool.SpawnParameters((Vector2)transform.position + _shootPos, Quaternion.identity, null, true));
            newMissile.GetComponent<Missile>().Initialize();
            _mainSource.Play();

            _missileTimer = 0f;
        }

        private void OnMove(float x) => _rigidbody.velocity = x * _moveSpeed * Vector2.right;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + _shootPos, Vector3.one * .05f);
        }
    }
}
