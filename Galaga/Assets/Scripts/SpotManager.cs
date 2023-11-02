using UnityEngine;

namespace Galaga
{
    public class SpotManager : MonoBehaviour
    {
        private const float HALF_PI = Mathf.PI / 2f;

        private static SpotManager _singleton;

        [SerializeField]
        private Spot[] _rightLeaningSpots, _leftLeaningSpots;

        private Vector2[] _rightStarts, _leftStarts;
        private Vector2[] _rightGoals, _leftGoals;

        [SerializeField]
        private float _frequency = 12f;

        private float _lerpTimer = 0f;
        private float _counter = 0f;

        private void Awake()
        {
            _singleton = this;

            float offset = .4f;
            _rightStarts = new Vector2[_rightLeaningSpots.Length];
            _rightGoals = new Vector2[_rightStarts.Length];
            _leftStarts = new Vector2[_leftLeaningSpots.Length];
            _leftGoals = new Vector2[_leftStarts.Length];
            for (int i = 0; i < _rightStarts.Length; i++)
            {
                _rightStarts[i] = _rightLeaningSpots[i].transform.position;
                _leftStarts[i] = _leftLeaningSpots[i].transform.position;
                _rightGoals[i] = _rightStarts[i] + new Vector2(offset, -offset);
                _leftGoals[i] = _leftStarts[i] + new Vector2(-offset, -offset);
            }
        }

        private void Update()
        {
            for (int i = 0; i < _rightLeaningSpots.Length; i++)
                _rightLeaningSpots[i].transform.position = Vector2.Lerp(_rightStarts[i], _rightGoals[i], _lerpTimer);

            for (int i = 0; i < _leftLeaningSpots.Length; i++)
                _leftLeaningSpots[i].transform.position = Vector2.Lerp(_leftStarts[i], _leftGoals[i], _lerpTimer);

            // Debug.Log(_lerpTimer);

            _lerpTimer = Mathf.Abs(Mathf.Sin(Mathf.PI * _counter / _frequency));
            _counter = (_counter + Time.deltaTime) % LoopThreshold();
        }

        private float LoopThreshold() => HALF_PI * (_frequency / Mathf.PI) * 2f;

        public static Spot GetAvailableSpot()
        {
            Spot[] rightSpots = _singleton._rightLeaningSpots;
            for (int i = 0; i < rightSpots.Length; i++)
                if (!rightSpots[i].IsFilled) return rightSpots[i];

            Spot[] leftSpots = _singleton._leftLeaningSpots;
            for (int i = 0; i < leftSpots.Length; i++)
                if (!leftSpots[i].IsFilled) return leftSpots[i];

            return null;
        }
    }
}
