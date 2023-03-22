using System;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Convert a Vector2 with integer values to an array.
    /// </summary>
    /// <param name="point">The Vector2 to convert.</param>
    /// <returns>An array with the X and Y coordinates of the Vector2.</returns>
    public static int[] ToArray(this Vector2 point)
    {
        return new int[2] { (int)point.x, (int)point.y };
    }
}
