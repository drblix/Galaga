using UnityEngine;
using Galaga.AI;

namespace Galaga
{
    public class Spot : MonoBehaviour
    {
        public Enemy FilledEnemy
        {
            get { return _enemy; }
            set { _enemy = value; }
        }

        public bool IsFilled
        {
            get { return _enemy != null; }
        }

        [SerializeField]
        private Enemy _enemy;
    }
}

