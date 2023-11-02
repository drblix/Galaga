using UnityEngine;

namespace Galaga
{
    [System.Serializable]
    public class BezierCurve
    {
        public CurveName Name
        {
            get { return _name; }
        }

        [SerializeField]
        private CurveName _name;

        public Vector2[] Controls
        {
            get { return _controls; }
        }

        [SerializeField]
        private Vector2[] _controls;

        public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            _controls = new Vector2[4];
            _controls[0] = p1;
            _controls[1] = p2;
            _controls[2] = p3;
            _controls[3] = p4;
        }

        public BezierCurve(Vector2[] controls)
        {
            if (controls.Length != 4) throw new System.ArgumentException("Bezier control point array must have 4 points!");

            _controls = controls;
        }

        public Vector2 Calculate(float t) => CubicLerp(_controls[0], _controls[1], _controls[2], _controls[3], t);

        public static Vector3 QuadraticLerp(Vector3 v1, Vector3 v2, Vector3 v3, float t)
        {
            Vector3 lerp1 = Vector3.Lerp(v1, v2, t);
            Vector3 lerp2 = Vector3.Lerp(v2, v3, t);

            return Vector3.Lerp(lerp1, lerp2, t);
        }

        public static Vector3 CubicLerp(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float t)
        {
            Vector3 lerp1 = QuadraticLerp(v1, v2, v3, t);
            Vector3 lerp2 = QuadraticLerp(v2, v3, v4, t);

            return Vector3.Lerp(lerp1, lerp2, t);
        }
    }

    public enum CurveName
    {
        CloseGrazeFromLeft,
        CloseGrazeFromRight,
        DivePlayer1
    }
}