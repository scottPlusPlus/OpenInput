using UnityEngine;


namespace GGS.IInput.Utils
{
    public static class VectorExtensions
    {
        public static Vector3 Sign(this Vector3 v1)
        {
            Vector3 v2 = Vector3.zero;
            if (v1.x < 0)
            {
                v2.x = -1;
            }
            else if (v1.x > 0)
            {
                v2.x = 1;
            }
            if (v1.y < 0)
            {
                v2.y = -1;
            }
            else if (v1.y > 0)
            {
                v2.y = 1;
            }
            if (v1.z < 0)
            {
                v2.z = -1;
            }
            else if (v1.z > 0)
            {
                v2.z = 1;
            }
            return v2;
        }

        public static bool IsNAN(this Vector3 v)
        {
            if (v.x != v.x || v.y != v.y || v.z != v.z)
            {
                return true;
            }
            return false;
        }
        
        public static bool Approximately(this Vector3 v, Vector3 v2, float tolerance)
        {
            return v.x.Approximately(v2.x, tolerance) &&
                    v.y.Approximately(v2.y, tolerance) &&
                    v.z.Approximately(v2.z, tolerance);
        }

        public static bool Approximately(this Vector3 v, Vector3 v2)
        {
            return v.Approximately(v2, Mathf.Epsilon);
        }
    }
}