using UnityEditor;
using UnityEngine;

namespace GalagaEditor
{
    [CustomEditor(typeof(BezierCurveVisualizer))]
    public class BezierCurveEditor : Editor
    {
        private int _arraySize = 50;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            BezierCurveVisualizer myTarget = target as BezierCurveVisualizer;

            EditorGUILayout.Space(35f);
            EditorGUILayout.LabelField("Bake Size");
            _arraySize = EditorGUILayout.IntSlider(_arraySize, 5, 200);
            if  (GUILayout.Button("Bake Positions"))
            {
                myTarget.BakePositions(_arraySize);
            }
        }
    }
}

