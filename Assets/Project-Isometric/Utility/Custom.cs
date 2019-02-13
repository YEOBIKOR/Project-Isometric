using UnityEngine;

namespace Custom
{
    public struct CustomMath
    {
        public static float Curve(float t, float u)
        {
            t = Mathf.Clamp01(t);

            if (u > 0f)
                return Mathf.Pow(t, 1f + u);
            else
                return 1f - Mathf.Pow(1f - t, 1f - u);
        }

        public static float ToAngle(float f)
        {
            return f - Mathf.Floor((f + 180f) / 360f) * 360f;
        }

        public static Vector2 Rotate(Vector2 v, float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radian);
            float cos = Mathf.Cos(radian);

            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        public static Vector3 HorizontalRotate(Vector3 v, float angle)
        {
            Vector2 r = Rotate(new Vector2(v.x, v.z), angle);

            return new Vector3(r.x, v.y, r.y);
        }

        public static float Factor(float time, float startTime, float duration)
        {
            return Mathf.Clamp01((time - startTime) / duration);
        }
    }

    public class PerlinNoise1D
    {
        private static float Noise(int x)
        {
            x = (x << 13) ^ x;
            return (1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7FFFFFFF) / 1073741824f);
        }

        private static float SmoothedNoise(int x)
        {
            return Noise(x) * 0.5f + (Noise(x - 1) + Noise(x + 1)) * 0.25f;
        }

        private static float Cerp(float a, float b, float t)
        {
            float ft = t * Mathf.PI;
            float f = (1f - Mathf.Cos(ft)) * 0.5f;

            return a * (1f - f) + b * f;
        }

        private static float InterpolatedNoise(float x)
        {
            int intX = Mathf.FloorToInt(x);
            float fracionalX = x - intX;

            return Cerp(SmoothedNoise(intX), SmoothedNoise(intX + 1), fracionalX);
        }

        public static float Get(float x, int octaves, float persistance)
        {
            float result = 0f;

            for (int i = 0; i < octaves; i++)
                result += InterpolatedNoise(x * i * 2f) * persistance * i;

            return result;
        }
    }
}
