using System.Collections;
using UnityEngine;

namespace Galaga
{
    public class StarBackground : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 3f;

        [SerializeField]
        private float _startingY, _yThreshold;

        private void Update()
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.down);

            if (transform.position.y < _yThreshold)
                transform.position = Vector3.up * _startingY;
        }
    }
}
