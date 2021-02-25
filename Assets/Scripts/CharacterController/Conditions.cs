using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A public class that contains static conditional functions
/// </summary>
public class Conditions
{
    /// <summary>
    /// Determines if a value is within a tolerance of a target
    /// </summary>
    /// <param name="value">The value to checl</param>
    /// <param name="target">The target value</param>
    /// <param name="tolerance">The tolerance</param>
    /// <returns>Returns true if the target has a difference from value of less than tolerance</returns>
    public static bool InTolerance(float value, float target, float tolerance)
    {   //Get the difference
        value -= target;
        //Check the tolerance
        if (Mathf.Abs(value) < tolerance)
            return true;

        return false;
    }
    /// <summary>
    /// Returns true if value is between or equal to min and max
    /// </summary>
    /// <param name="value">The value to compare</param>
    /// <param name="min">The min value</param>
    /// <param name="max">The maximum value</param>
    /// <returns>Returns true if its in range</returns>
    public static bool InRage(float value, float min, float max)
    {
        if (value >= min && value <= max)
            return true;
        return false;
    }
    /// <summary>
    /// Compares two raycasts
    /// </summary>
    /// <param name="x">Raycasts X to compare</param>
    /// <param name="y">Raycasts Y to compare</param>
    /// <returns>Returns 0 if equal distance, -1 if X is closer, 1 if Y is closer</returns>
    public static int CompareDist(RaycastHit x, RaycastHit y)
    {
        if (x.distance == y.distance)
            return 0;
        if (x.distance < y.distance)
            return -1;
        return 1;
    }
}
