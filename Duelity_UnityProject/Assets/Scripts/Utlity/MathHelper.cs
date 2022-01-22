using UnityEngine;

namespace Duelity.Utility
{
    public static class MathHelper
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static Vector2 GetPointOnCircle(Vector2 origin, float radius, float angleInRadians)
        {
            float x = Mathf.Cos(angleInRadians) * radius;
            float y = Mathf.Sin(angleInRadians) * radius;
            return origin + new Vector2(x, y);
        }

        public static bool PointIsInsideCircle(Vector2 point, Vector2 circleOrigin, float circleRadius)
        {
            return (Vector2.Distance(circleOrigin, point)) <= circleRadius;
        }

        public static bool RoughlyEqual(this float a, float b, float margin = 0.01f)
        {
            return Mathf.Abs(a - b) <= margin;
        }
    }
}