using UnityEngine;

//Script for universal methods here
public class Utilities : MonoBehaviour
{
    /// <summary>
    /// Returns the average number by adding all values together and dividing that by the total number of values
    /// </summary>
    /// <param name="vector">The vector containing the values</param>
    /// <returns>The Average Number of the vector</returns>
    // Very useful for getting the overall scale of an object
    static public float Vec3Average(Vector3 vector)
    {
        return (vector.x + vector.y + vector.z) / 3;
    }

    //== Bounds =======================================================\\
    /// <summary>
    /// Returns a random point from a specified bound
    /// Note: this is a local point within the gameObject.
    /// </summary>
    /// <param name="bounds">The bounds that will be read and set a return value based on it's size</param>
    /// <returns>A random local point in the bounds</returns>
    static public Vector3 GetRandomPointInBounds(Bounds bounds)
    {
        float X = bounds.size.x * -0.5f;
        float Y = bounds.size.y * -0.5f;
        float Z = bounds.size.z * -0.5f;

        return new Vector3(
            Random.Range(-X, X),
            Random.Range(-Y, Y),
            Random.Range(-Z, Z)
            );
    }
}
