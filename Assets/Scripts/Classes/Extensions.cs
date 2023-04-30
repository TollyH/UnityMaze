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

    public static void MoveAbsolute(this CharacterController character, Vector3 position)
    {
        character.enabled = false;
        character.transform.position = position;
        character.enabled = true;
    }

    public static void DestroyAllChildren(this GameObject gameObject)
    {
        while (gameObject.transform.childCount > 0)
        {
            Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
        }
    }

    public static Vector2 ToMazePosition(this Vector3 position, float unitSize)
    {
        return new((-position.x + (unitSize / 2)) / unitSize,
            (position.z + (unitSize / 2)) / unitSize);
    }
}
