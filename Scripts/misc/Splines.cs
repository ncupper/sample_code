using UnityEngine;
namespace misc
{
    internal class Spline
    {
        private readonly Vector3[] _points = new Vector3[4];

        public void Reset()
        {
        }

        public void SetPoint(int idx, Vector3 pos)
        {
            _points[idx] = pos;
        }

        public Vector3 GetHermite(float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            var tension = 0.5f;

            Vector3 T1 = tension * (_points[2] - _points[0]);
            Vector3 T2 = tension * (_points[3] - _points[1]);

            float Blend1 = 2 * t3 - 3 * t2 + 1;
            float Blend2 = -2 * t3 + 3 * t2;
            float Blend3 = t3 - 2 * t2 + t;
            float Blend4 = t3 - t2;

            return Blend1 * _points[1] + Blend2 * _points[2] + Blend3 * T1 + Blend4 * T2;
        }
    }
}
