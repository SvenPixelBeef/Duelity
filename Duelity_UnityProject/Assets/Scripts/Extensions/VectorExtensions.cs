using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;

        tx = (cos * tx) - (sin * ty);
        ty = (sin * tx) + (cos * ty);

        return new Vector2(tx, ty);
    }

    /// <summary>
    /// Snap this vector to the nearest value specified by the snapIncrement.
    /// Only supports values from 0 - 1.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="snapIncrement"></param>
    /// <returns></returns>
    public static Vector2 Snap(this Vector2 vector, float snapIncrement = .25f)
    {
        snapIncrement = Mathf.Clamp01(snapIncrement);
        return new Vector2(vector.x.Snap(snapIncrement), vector.y.Snap(snapIncrement));
    }

    /// <summary>
    /// Snap this vector to the nearest value specified by the snapIncrement.
    /// Only supports values from 0 - 1.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="snapIncrement"></param>
    /// <returns></returns>
    public static Vector3 Snap(this Vector3 vector, float snapIncrement = .1f)
    {
        snapIncrement = Mathf.Clamp01(snapIncrement);
        return new Vector3(vector.x.Snap(snapIncrement), vector.y.Snap(snapIncrement), vector.z.Snap(snapIncrement));
    }
}
