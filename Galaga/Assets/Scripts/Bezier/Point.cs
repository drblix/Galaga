using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaga
{
    [System.Serializable]
    public struct Point
    {
        public readonly Vector2 Position
        {
            get { return _position; }
        }

        public Vector2 Direction
        {
            readonly get { return _direction; }
            set { _direction = value; }
        }

        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private Vector2 _direction;

        public Point(Vector2 position)
        {
            _position = position;
            _direction = Vector2.zero;
        }

    }
}
