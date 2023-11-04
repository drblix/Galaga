using Galaga;
using UnityEngine;

namespace GalagaEditor
{
    public class BezierCurveVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _handles;

        [SerializeField]
        private Point[] _points;

        [Header("Gizmo Settings")]

        [SerializeField]
        private float _gizmoSize = .2f;

        [SerializeField]
        private Color _gizmoColor = Color.red;


        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;

            for (int i = 0; i < _handles.Length; i++)
            {
                Gizmos.DrawWireCube(_handles[i].position, Vector3.one * _gizmoSize);
            }

            for (float t = 0f; t < 1f; t += .025f)
            {
                Vector3 pos = BezierCurve.CubicLerp(_handles[0].position, _handles[1].position, _handles[2].position, _handles[3].position, t);

                Gizmos.DrawSphere(pos, .05f);
            }
        }

        public void BakePositions(int size)
        {
            _points = new Point[size];
            for (int i = 0; i < size; i++)
            {
                float t = (float)(i+1) / size;

                Vector3 pos = BezierCurve.CubicLerp(_handles[0].position, _handles[1].position, _handles[2].position, _handles[3].position, t);
                _points[i] = new Point(pos);
            }
        }
    }
}

