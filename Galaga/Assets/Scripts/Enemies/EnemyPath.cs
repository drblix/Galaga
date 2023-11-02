using UnityEngine;

namespace Galaga
{
    [CreateAssetMenu(fileName = "Path", menuName = "Galaga/EnemyPath")]
    public class EnemyPath : ScriptableObject
    {
        public Point[] Path
        {
            get { return _path; }
        }

        [SerializeField]
        private Point[] _path;
    }
}

