using System.Collections.Generic;
using UnityEngine;

namespace Galaga
{
    public class PathContainer : MonoBehaviour
    {
        public static PathContainer Singleton
        {
            get { return _singleton; }
        }

        private static PathContainer _singleton;

        public List<BezierCurve> Curves
        {
            get { return _curves; }
        }

        [SerializeField]
        private List<BezierCurve> _curves;


        public int PointNumber
        {
            get { return _pointNum; }
        }

        [Header("Gizmo Settings")]

        [SerializeField]
        [Range(1, 100)]
        private int _pointNum = 20;

        private void Awake()
        {
            _singleton = this;
        }

        public BezierCurve GetCurve(int index) => _curves[index];
        public BezierCurve GetCurve(CurveName name)
        {
            for (int i = 0; i < _curves.Count; i++)
                if (_curves[i].Name == name) return _curves[i];

            throw new System.Exception($"{name} curve does not in the array!");
        }

        private void OnDrawGizmos() => PathContainerEditor.DrawGizmos(this);
    }

}
