using UnityEditor;
using UnityEngine;

namespace Galaga
{
    [CustomEditor(typeof(PathContainer))]
    public class PathContainerEditor : Editor
    {
        private static bool[] _visiblities;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PathContainer thisTarget = target as PathContainer;

            if (thisTarget == null) return;
            _visiblities ??= new bool[thisTarget.Curves.Count];

            // creates new array if number of curves has been updated in the inspector
            if (_visiblities.Length != thisTarget.Curves.Count)
            {
                bool[] lastArr = _visiblities;
                _visiblities = new bool[thisTarget.Curves.Count];

                // updates new array with previous values
                for (int i = 0; i < lastArr.Length; i++)
                {
                    if (i >= _visiblities.Length || i >= lastArr.Length) break;

                    _visiblities[i] = lastArr[i];
                }
            }

            EditorGUILayout.Space(30f);
            EditorGUILayout.HelpBox("Toggle Visibility of Curves In Scene View", MessageType.None);
            for (int i = 0; i < thisTarget.Curves.Count; i++)
            {
                _visiblities[i] = EditorGUILayout.Toggle($"Show Curve {i + 1}", _visiblities[i]);
            }

            //DrawGizmos(thisTarget);
        }

        public static void DrawGizmos(PathContainer thisTarget)
        {
            if (thisTarget == null || _visiblities == null) return;

            for (int i = 0; i < thisTarget.Curves.Count; i++)
            {
                if (!_visiblities[i]) continue;

                BezierCurve curve = thisTarget.GetCurve(i);

                Gizmos.color = Color.yellow;
                for (int j = 0; j < curve.Controls.Length; j++)
                    Gizmos.DrawWireCube(curve.Controls[j], Vector3.one * .5f);

                Gizmos.color = Color.red;

                float increment = 1f / thisTarget.PointNumber;
                if (increment < 0f) throw new System.Exception("Increment is less than 1! Prevented infinite loop!");

                for (float x = 0f; x < 1f; x += increment)
                    Gizmos.DrawSphere(curve.Calculate(x), .15f);
            }
        }
    }
}

