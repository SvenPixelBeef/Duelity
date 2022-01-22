using UnityEngine;

public static class FloatExtensions
{
    /// <summary>
    /// Snap this float to the nearest value specified by the snapIncrement.
    /// Only supports values from 0 - 1.
    /// </summary>
    /// <param name="f"></param>
    /// <param name="snapIncrement"></param>
    /// <returns></returns>
    public static float Snap(this float f, float snapIncrement = .25f)
    {
        snapIncrement = Mathf.Clamp01(snapIncrement);
        int wholeNumberX = (int)f;
        float decimalPartX = wholeNumberX == 0 ? f : f % wholeNumberX;
        return wholeNumberX + (decimalPartX - (decimalPartX % snapIncrement));
    }
}
