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

        [SerializeField]
        private Vector2 _position;

        public Point(Vector2 position)
        {
            _position = position;
        }

    }
}
