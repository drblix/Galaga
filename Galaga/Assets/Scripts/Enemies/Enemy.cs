using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Galaga
{
    namespace AI
    {
        public abstract class Enemy : PoolableGameObject, IDestroyable
        {
            public EnemyState State
            {
                get { return _state; }
            }

            [SerializeField]
            protected EnemyState _state;

            [SerializeField]
            protected AudioSource _source;
            [SerializeField]
            protected Animator _animator;
            [SerializeField]
            protected Collider2D _collider;

            [SerializeField]
            protected Spot _assignedSpot;

            [SerializeField]
            protected float _missileShootingHeight = -5f;

            protected static readonly int _explosionId = Animator.StringToHash("Explosion");

            protected bool _hit = false;
            protected float _formationTimer = 0f;
            protected float _diveThreshold;

            protected float _missileThreshold;
            protected float _missileTimer = 0f;

            protected virtual void Update()
            {
                if (_state == EnemyState.Formation)
                {
                    if (_assignedSpot != null)
                    {
                        transform.position = _assignedSpot.transform.position;
                    }

                    if (_formationTimer > _diveThreshold)
                    {
                        if (RoundManager.Singleton.CanDive())
                            DivePlayer(3.5f);
                        else
                            _formationTimer = 0f;
                    }
                    else
                        _formationTimer += Time.deltaTime;
                }

                if (_state == EnemyState.Diving)
                {
                    if (_missileTimer > _missileThreshold && transform.position.y > _missileShootingHeight)
                    {
                        // create new missile to shoot at player
                        GameObject newMissile = ObjectPool.GetObject(PooledObject.EnemyMissile, new ObjectPool.SpawnParameters(transform.position, Quaternion.identity, null, true));
                        newMissile.GetComponent<Missile>().Initialize();

                        _missileTimer = 0f;
                        _missileThreshold = Random.Range(.5f, 3.5f);
                    }

                    _missileTimer += Time.deltaTime;
                }
            }

            public virtual void Hit()
            {
                StopAllCoroutines();

                _hit = true;
                _collider.enabled = false;
                _animator.SetTrigger(_explosionId);

                _source.Play();

                RoundManager.Singleton.SpawnedEnemies.Remove(this);
                ObjectPool.PoolObject(this, _animator.GetCurrentAnimatorStateInfo(0).length);
            }

            public override void OnSpawn()
            {
                _hit = false;
                _collider.enabled = true;
                _diveThreshold = Random.Range(3.5f, 10f);
                _missileThreshold = Random.Range(1f, 3.5f);

                RoundManager.Singleton.SpawnedEnemies.Add(this);
            }


            // Spawn enemy and have them fly the specified path
            // Upon the enemy finishing the path, fly towards the specified spot and assign it
            public void SpawnSequence(BezierCurve curve, float pathDur, float toSpotDur) => StartCoroutine(SpawnSequenceRoutine(curve, pathDur, toSpotDur));

            protected IEnumerator SpawnSequenceRoutine(BezierCurve curve, float pathDuration, float toSpotDuration)
            {
                Spot spot = SpotManager.GetAvailableSpot();
                spot.FilledEnemy = this;

                // wait for enemy to fly path
                yield return FlyPath(curve, pathDuration);

                // fly towards spot
                yield return FlyTo(spot.transform.position, toSpotDuration);
                _assignedSpot = spot;
                _state = EnemyState.Formation;

                // face upwards
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            // Set state to diving
            // Follow a generated curve down towards the player, firing missiles occasionally
            public void DivePlayer(float diveSpeed) => StartCoroutine(DivePlayerRoutine(diveSpeed));

            protected IEnumerator DivePlayerRoutine(float diveSpeed)
            {
                _state = EnemyState.Diving;
                // generate curve that starts at enemy's location and ends at player's

                Vector2 control1 = new(Random.Range(-12.5f, 12.5f), Random.Range(10f, 20f));
                Vector2 control2 = new(Random.Range(-18.5f, 18.5f), Random.Range(-13f, 0f));
                //Vector2 endPosition = FindAnyObjectByType<Fighter>().transform.position;
                Vector2 endPosition = new(-control2.x / 2f, -16.75f);
                BezierCurve flightCurve = new (transform.position, control1, control2, endPosition);

                yield return FlyPath(flightCurve, diveSpeed);

                Debug.Log($"finished diving! {name}");

                // move back to spot from top of screen
                transform.position = new Vector2(_assignedSpot.transform.position.x, 19f);
                yield return FlyTo(_assignedSpot.transform.position, 1f);

                _state = EnemyState.Formation;
                _formationTimer = 0f;
            }

            public IEnumerator FlyTo(Vector2 destination, float duration)
            {
                Vector2 start = transform.position;
                float timer = 0f;
                transform.rotation = Quaternion.LookRotation(Vector3.forward, destination - start);

                while (timer / duration < 1f)
                {
                    timer += Time.deltaTime;
                
                    Vector2 lerpedVector = Vector2.Lerp(start, destination, timer / duration);
                    transform.position = lerpedVector;

                    yield return new WaitForEndOfFrame();
                }

                transform.position = destination;
                Debug.LogWarning("done moving! " + transform.name);
            }

            /// <summary>
            /// Commands the enemy to follow a bezier curve path
            /// </summary>
            public IEnumerator FlyPath(BezierCurve curve, float duration)
            {
                float timer = 0f;

                while (timer / duration < 1f)
                {
                    Vector2 posNow = curve.Calculate(timer / duration);
                    Vector2 nextPos = curve.Calculate((timer + Time.deltaTime) / duration);
                    transform.SetPositionAndRotation(posNow, Quaternion.LookRotation(Vector3.forward, nextPos - posNow));

                    timer += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }

            private void OnCollisionEnter2D(Collision2D collision)
            {
                if (collision.collider != null && collision.collider.TryGetComponent(out IDestroyable destroyable))
                    destroyable.Hit();
            }
        }

        public enum EnemyState
        {
            Spawning,
            Diving,
            Formation
        }
    }
}

