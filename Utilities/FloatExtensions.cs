using UnityEngine;


namespace GGS.OpenInput.Utils
{
    public static class FloatExtensions
    {

        public static float ReMap(this float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static bool Approximately(this float f, float f2, float tolerance)
        {
            if (tolerance < 0f)
            {
                Debug.LogError("Tolerance must be a positive value");
                return false;
            }

            return Mathf.Abs(f - f2) < tolerance;
        }

        public static bool Approximately(this float f, float f2)
        {
            return f.Approximately(f2, Mathf.Epsilon);
        }

    }
}

