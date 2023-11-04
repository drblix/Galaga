using System.Collections;
using System.Collections.Generic;
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

            protected static readonly int _explosionId = Animator.StringToHash("Explosion");

            protected bool _hit = false;
            protected float _formationTimer = 0f;
            protected float _diveThreshold;

            protected ushort _missilesToShoot = 0;
            protected ushort _minMissiles = 0, _maxMissiles = 3;
            protected readonly List<float> _firingTimes = new();

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
                // _missileThreshold = Random.Range(1f, 3.5f);

                RoundManager.Singleton.SpawnedEnemies.Add(this);
            }


            // Spawn enemy and have them fly the specified path
            // Upon the enemy finishing the path, fly towards the specified spot and assign it
            public void SpawnSequence(BezierCurve curve, float pathDur, float toSpotDur) => StartCoroutine(SpawnSequenceRoutine(curve, pathDur, toSpotDur));

            protected IEnumerator SpawnSequenceRoutine(BezierCurve curve, float pathDuration, float toSpotDuration)
            {
                Spot spot = SpotManager.GetAvailableSpot(this);
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
                Vector2 endPosition = new(-control2.x / 2f, -16.75f);
                BezierCurve flightCurve = new (transform.position, control1, control2, endPosition);

                // define times when to fire missiles
                ushort numToAdd = (ushort)Random.Range(_minMissiles, _maxMissiles);
                _firingTimes.Clear();

                for (ushort i = 0; i < numToAdd; i++)
                    _firingTimes.Add(Random.Range(.1f, .65f));

                _firingTimes.Sort();

                yield return FlyPath(flightCurve, diveSpeed);

                Debug.Log($"finished diving! {name}");

                // move back to spot from top of screen
                transform.position = new Vector2(_assignedSpot.transform.position.x, 19f);
                yield return FlyTo(_assignedSpot.transform.position, 1f);

                transform.rotation = Quaternion.Euler(Vector3.zero);
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
                    float lerp = timer / duration;

                    Vector2 posNow = curve.Calculate(lerp);
                    Vector2 nextPos = curve.Calculate((timer + Time.deltaTime) / duration);
                    transform.SetPositionAndRotation(posNow, Quaternion.LookRotation(Vector3.forward, nextPos - posNow));

                    // checking if a missile should be fired at this point in the path
                    if (_firingTimes.Count > 0)
                    {
                        if (lerp > _firingTimes[0])
                        {
                            GameObject missile = ObjectPool.GetObject(PooledObject.EnemyMissile, new ObjectPool.SpawnParameters(transform.position, Quaternion.identity, null, true));
                            missile.GetComponent<Missile>().Initialize();

                            _firingTimes.RemoveAt(0);
                        }
                    }

                    /*
                    for (int i = 0; i < _firingTimes.Count; i++)
                    {
                        if (timer / duration > _firingTimes[i])
                        {
                            GameObject missile = ObjectPool.GetObject(PooledObject.EnemyMissile, new ObjectPool.SpawnParameters(transform.position, Quaternion.identity, null, true));
                            missile.GetComponent<Missile>().Initialize();

                            _firingTimes.RemoveAt(i);
                        }
                    }
                    */

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

