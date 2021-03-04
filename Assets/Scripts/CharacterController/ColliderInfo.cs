using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Stores the players capsual collider information with related access and manipulation functions
/// </summary>
[System.Serializable]
public struct ColliderInfo
{
    /// <summary>
    /// A reference to the players transform
    /// </summary>
    private Transform origin;
    /// <summary>
    /// The radius of the capsual
    /// </summary>
    [SerializeField]
    [Tooltip("The radius of the capsual")]
    private float radius;
    /// <summary>
    /// How far below the origin the capsual extends. Includes radius
    /// </summary>
    [SerializeField]
    [Tooltip("How far below the origin the capsual extends. Includes radius.")]
    private float lowerHeight;
    /// <summary>
    /// How far above the origin the capsual extends. Includes radius.
    /// </summary>
    [SerializeField]
    [Tooltip("How far above the origin the capsual extends. Includes radius")]
    private float upperHeight;
    /// <summary>
    /// The distance away from any surface the collider should remain. CANNOT BE LESS THAN OR EQUAL TO 0
    /// </summary>
    [SerializeField]
    [Range(0.00001f, 1f)]
    [Tooltip("The distance away from any surface the collider should remain. Cannot be LESS THAN OR EQUAL TO 0")]
    private float collisionOffset;
    /// <summary>
    /// The angle at which a slope is considered ground the player can walk up.
    /// </summary>
    [Tooltip("The angle of a slope at which the player can walk up. Angles less than this value are considered ground")]
    [SerializeField]
    [Range(0, 90)]
    private float slopeAngle;
    /// <summary>
    /// Determines wether the position offset of the collider should be read as local or global co-ordinates
    /// TLDR: Should positionOffset be affected by origin transform rotation.
    /// </summary>
    [Tooltip("Should Position Offset of the collider be affected by origin transform rotation.")]
    [SerializeField]
    private bool positionOffsetIsGlobal;
    /// <summary>
    /// Determines if the rotation of the collider is in local or global co-ordinates
    /// </summary>
    [Tooltip("Should orienation be in local or global co-ordinates")]
    [SerializeField]
    private bool rotationIsGlobal;
    /// <summary>
    /// The positional offset from the origin of the collider
    /// </summary>
    private Vector3 posOffset;
    /// <summary>
    /// The orientation of the collider. Orientation points towards the top of the capsual
    /// </summary>
    private Vector3 orientation;
    /// <summary>
    /// The direction of gravity
    /// </summary>
    private Vector3 gravityDir;
    /// <summary>
    /// Stores if the Collider is on the ground or not
    /// </summary>
    private bool onGround;
    /// <summary>
    /// Sets reference to the transform to the given transform.
    /// </summary>
    /// <param name="t">The transform to be set</param>
    public void SetTransform(Transform t)
    {
        origin = t;
        //Spit out an error if the transform is invalid
        CheckValidTransform();
    }
    /// <summary>
    /// Gets or Sets the Orientation. If Orientation is zero, -Gravity will be returned instead
    /// </summary>
    public Vector3 Orientation
    {
        get
        {   //If orientation is 0, return -gravity
            if (orientation == Vector3.zero)
                return -GravityDirection;
            //If the rotation is global, return the rotation
            if (rotationIsGlobal)
                return orientation;

            //Return orientation based on origins rotation
            return (origin.forward * orientation.z
                + origin.right * orientation.x
                + origin.up * orientation.y).normalized;
        }
        set
        {
            orientation = value.normalized;
        }
    }
    /// <summary>
    /// Gets or Sets the direction of gravity. If gravityDir is zero, (0, -1, 0) is returned
    /// </summary>
    public Vector3 GravityDirection
    {
        get
        {   //If gravityDir is 0 or unassigned return(0, -1, 0)
            if (gravityDir == Vector3.zero)
                return Vector3.down;
            return gravityDir;
        }
        set
        {
            gravityDir = value.normalized;
        }
    }
    /// <summary>
    /// Gets or Sets the positional offset from the transform of the collider
    /// </summary>
    public Vector3 PositionOffset
    {
        get
        {
            return posOffset;
        }
        set
        {
            posOffset = value;
        }
    }

    #region GetPoints
    /// <summary>
    /// Returns the position of the sphere that represents the lower part of the capsual
    /// </summary>
    /// <returns>Returns the position of the sphere that represents the lower part of the capsual</returns>
    public Vector3 GetLowerPoint()
    {   //Call get LowestPoint but with and offset of -Radius
        return GetLowestPoint(-Radius);
    }
    /// <summary>
    /// Returns the position of the sphere that represents the lower part of the capsual.
    /// The offset is added onto lowerHeight before calculating the point
    /// </summary>
    /// <param name="offset">An offset applied to lowerHeight before the point is calculated</param>
    /// <returns>Returns the position of the sphere that represents the lower part of the capsual</returns>
    public Vector3 GetLowerPoint(float offset)
    {   //Call Get Lowest Point with an offset of offset & -Radius
        return GetLowestPoint(-Radius + offset);
    }
    /// <summary>
    /// Returns the position at the bottom of the capsual
    /// </summary>
    /// <returns>The point at the bottom most point of the capsual</returns>
    public Vector3 GetLowestPoint()
    {   //Call GetLowestPoint with an offset of 0
        return GetLowestPoint(0);
    }
    /// <summary>
    /// Returns the point at the bottom of the capsual
    /// </summary>
    /// <param name="offset">An offset applied to lowerHeight before the point is calculated</param>
    /// <returns>The bottom most point of the capsual</returns>
    public Vector3 GetLowestPoint(float offset)
    {   //Make sure the transform is valid, otherwise return 0
        if (CheckValidTransform())
            return Vector3.zero;
        //Calculate the location of the lower circle
        return GetOriginPosition() - Orientation * (LowerHeight + offset);
    }
    /// <summary>
    /// Returns the point of the sphere that reprsents the top of the capsual
    /// </summary>
    /// <returns>The point the sphere that makes up the top of the capsual exists at</returns>
    public Vector3 GetUpperPoint()
    {   //Keep all the math in one place
        return GetHighestPoint(-Radius);
    }
    /// <summary>
    /// Returns the point of the sphere that reprsents the top of the capsual
    /// Offset is added to upperHeight before calculating the point
    /// </summary>
    /// <param name="offset">An offset applied to upperHeight before calculating the point</param>
    /// <returns>The point the sphere that makes up the top of the capsual exists at</returns>
    public Vector3 GetUpperPoint(float offset)
    {   //Keep all the math in one place
        return GetHighestPoint(-Radius + offset);
    }
    /// <summary>
    /// Returns the point at the top of the capsual
    /// </summary>
    /// <returns>The upper most point of the capsual</returns>
    public Vector3 GetHighestPoint()
    {   //Keep all the math in one place
        return GetHighestPoint(0);
    }
    /// <summary>
    /// Returns the point at the top of the capsual
    /// Offset is added to UpperPoint before calculation
    /// </summary>
    /// <param name="offset">The offset to upperPoint</param>
    /// <returns>The upper most point of the capsual with an offset</returns>
    public Vector3 GetHighestPoint(float offset)
    {   //Make sure the transform is valid
        if (CheckValidTransform())
            return Vector3.zero;
        //Calculate the point
        return GetOriginPosition() + Orientation * (UpperHeight + offset);
    }
    #endregion
    /// <summary>
    /// A Get Set for the radius of this capsual
    /// </summary>
    public float Radius
    {
        get
        {   //If radius is 0. Report an error
            if (radius == 0)
                Debug.LogError("Radius is 0");
            return radius;
        }
        set
        {   //Make sure the value is a positive or 0 number
            if (value < 0)
                value = 0;
            radius = value;
        }
    }
    /// <summary>
    /// Returns the true radius of the player. The Radius + the collision Offset
    /// </summary>
    public float TrueRadius
    {
        get
        {
            return Radius + CollisionOffset;
        }
    }
    /// <summary>
    /// A Get Set for the Upper Height of the capsual.
    /// Upper Height is the distance above the origin the capsual extends
    /// </summary>
    public float UpperHeight
    {
        get
        {
            return upperHeight;
        }
        set
        {
            upperHeight = value;
        }
    }
    /// <summary>
    /// A Get Set for the Lower Height of the capsual.
    /// Lower Height is the distance below the origin the capsual extends
    /// </summary>
    public float LowerHeight
    {
        get
        {
            return lowerHeight;
        }
        set
        {
            lowerHeight = value;
        }
    }
    /// <summary>
    /// Returns the total height of the capsual
    /// </summary>
    public float Height
    {
        get
        {
            return lowerHeight + upperHeight;
        }
    }
    /// <summary>
    /// A Get Set for the collision offset.
    /// Collision Offset is the distance the collider must be from any surface
    /// </summary>
    public float CollisionOffset
    {
        get
        {   //If collisionOffset is 0, or worse, negative, return 0.01f.
            if (collisionOffset <= 0)
                return 0.01f;
            return collisionOffset;
        }
        set
        {   //If value is < 0. Set collision offset to 0.01f
            if (value < 0)
                value = 0.01f;
            collisionOffset = value;
        }
    }
    /// <summary>
    /// A Get Set for the slopeAngle
    /// SlopeAngle is the angle of a surface that the player can walk up and is considered ground
    /// </summary>
    public float SlopeAngle
    {
        get
        {
            return slopeAngle;
        }
        set
        {   //Make sure value is between 0 and 90 degrees
            if (value > 0 && value < 90)
                slopeAngle = value;
        }
    }
    /// <summary>
    /// A Get Set for OnGround. Is automatically set when MoveTo is called on the PlayerController class
    /// </summary>
    public bool OnGround
    {
        get
        {
            return onGround;
        }
        set
        {
            onGround = value;
        }
    }
    /// <summary>
    /// Toggle whether position offset is global
    /// </summary>
    public bool PositionOffsetIsGlobal
    {
        get { return positionOffsetIsGlobal; }
        set { positionOffsetIsGlobal = value; }
    }
    /// <summary>
    /// Toggle whether orientation is global
    /// </summary>
    public bool OrientationIsGlobal
    {
        get { return rotationIsGlobal; }
        set { rotationIsGlobal = value; }
    }
    /// <summary>
    /// Determines if a slope given by a normal can be stood on or is too steep
    /// </summary>
    /// <param name="normal">The normal of the slope</param>
    /// <returns>Returns true if the player can stand on the surface</returns>
    public bool ValidSlope(Vector3 normal)
    {
        if (Vector3.Dot(normal, -GravityDirection) >= Mathf.Cos(SlopeAngle * Mathf.Deg2Rad))
            return true;
        return false;
    }
    /// <summary>
    /// Gets the origin of this collider.
    /// Gets origins position offset by positionOffset
    /// </summary>
    /// <returns>Returns the location of the capsual collider</returns>
    public Vector3 GetOriginPosition()
    {   //Make sure the transform is valid
        if (CheckValidTransform())
            return Vector3.zero;
        if (!positionOffsetIsGlobal)
            return origin.position + origin.right * posOffset.x + origin.up * posOffset.y + origin.forward * posOffset.z;
        else
            return origin.position + posOffset;
    }
    /// <summary>
    /// Checks if the transform locally stored is valid
    /// </summary>
    /// <returns>Returns true if this transform is invalid</returns>
    private bool CheckValidTransform()
    {   //If origin is null, report and error and break
        if (origin == null)
        {
            Debug.LogError("Transform not assigned to collider info");
            Debug.Break();
            return true;
        }
        return false;
    }
    /// <summary>
    /// Determines the distance to the edge of the capsual from a direction dir
    /// </summary>
    /// <param name="dir">The direction to get the distance of</param>
    /// <returns>Returns the distance to the edge</returns>
    public float DistanceToEdge(Vector3 dir)
    {
        return DistanceToEdgeGivenRadius(dir, Radius);
    }
    /// <summary>
    /// Determines the distance to the edge of the capsual from a direction dir with collisionOffset
    /// </summary>
    /// <param name="dir">The direction to get the distance of</param>
    /// <returns>Returns the distance to the edge</returns>
    public float DistanceToEdgeWithOffset(Vector3 dir)
    {
        return DistanceToEdgeGivenRadius(dir, Radius + CollisionOffset);
    }
    /// <summary>
    /// Calculates the distance to the edge from a direction
    /// </summary>
    /// <param name="dir">The direction of the check</param>
    /// <param name="radius">The radius of the capsual</param>
    /// <returns>Returns the distance to the edge</returns>
    private float DistanceToEdgeGivenRadius(Vector3 dir, float radius)
    {
        dir.Normalize();
        float dot = Vector3.Dot(dir, Orientation);

        //If dot is > 0, then we need to check against the upper point.
        if (dot > 0)
            return DistanceToEdgeMath(dir, radius, GetUpperPoint());
        //If dot is < 0, then we need to check against the lower point, instead of upper point.
        else if (dot < 0)
            return DistanceToEdgeMath(dir, radius, GetLowerPoint());
        //Return the radius
        return radius;
    }
    /// <summary>
    /// The maths used by DistanceToEdgeGivenRadius
    /// </summary>
    /// <param name="dir">The direction of the line</param>
    /// <param name="radius">The radius of the capsual</param>
    /// <param name="circlePoint">The point the circle occupies</param>
    /// <returns>Returns the distance to the edge</returns>
    private float DistanceToEdgeMath(Vector3 dir, float radius, Vector3 circlePoint)
    {
        Vector3 sPoint = GetOriginPosition();
        //Get an orientation vector
        Vector3 ori = circlePoint - sPoint;

        //First we need to figure out if we are exiting in the middle bit or the circle ends.
        Vector3 cross = Vector3.Cross(ori.normalized, dir);
        cross = (circlePoint + (cross.normalized * radius));
        cross -= sPoint;

        float dot = Vector3.Dot(dir.normalized, ori.normalized);
        float otherDot = Vector3.Dot(cross.normalized, ori.normalized);
        //If the dot is greater than the cross, then we need to find the intersection point in the circle.
        if (dot > otherDot)
        {
            //1.Get a Vector "toC" from the origin to the center circle that make up that end of the capsual
            Vector3 toC = circlePoint - sPoint;
            //2.Dot "toC" onto direction.
            dot = Vector3.Dot(dir.normalized, toC);
            //3.Assuming "toC" is the hypotenuse and the adjacent is the Dot we just calculated, get the opposite.
            float opposite = Mathf.Sqrt((toC.magnitude * toC.magnitude) - (dot * dot));
            //4.Assuming the opposite we just calculated is the opposite and the radius of the circle is the hypotenuse, get the adjacent.
            opposite = Mathf.Sqrt((radius * radius) - (opposite * opposite));
            //5. return the Adjacent + Dot
            return dot + opposite;
        }
        //Otherwise we exit in the middle bit so we can use the dot and some unit circle math to resize dir to be what we want
        else
        {
            //First we need to get the length of the Opposite (dot is Adjacent)
            float opposite = Mathf.Sqrt(1 - (dot * dot));

            //Scale up Opposite so its length is radius
            float scale = radius / opposite;

            //Scale up Adjacent by the same amount
            dot *= scale;
            //A^2 + B^2 = C^2
            //return C
            return Mathf.Sqrt((dot * dot) + (opposite * opposite));
        }
    }
    /// <summary>
    /// Calculates the point a given direction with origin would point to.
    /// Is used for determining collision response.
    /// </summary>
    /// <param name="lineOrigin">The origin of the line</param>
    /// <param name="lineDir">The direction of the line</param>
    /// <returns>Returns the point within the collider it would hit</returns>
    public Vector3 GetCenteralPoint(Vector3 lineOrigin, Vector3 lineDir)
    {   //Make sure lineDir is normalized
        lineDir.Normalize();
        //Get the dot onto orientation
        float dot = Vector3.Dot(lineDir, Orientation);
        //If the dot is less than 0, then the direction must hit the upper point
        if (dot < 0)
            return GetUpperPoint();
        //If its > 0 then it must hit the lower point
        else if (dot > 0)
            return GetLowerPoint();
        //Get a vector from the origin to the lines origin and dot it onto orientation
        Vector3 oToLO = lineOrigin - GetOriginPosition();
        dot = Vector3.Dot(oToLO, Orientation);
        //Move the origin of the collider along orientation by the dot to get the point it would hit
        return GetOriginPosition() + (Orientation * dot);
    }


    #region Static Functions
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <returns>Returns true if something was hit</returns>
    public static bool Cast(ColliderInfo c, Vector3 castVec)
    {
        return Cast(c, castVec, Vector3.zero, out _);
    }
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <param name="offset">The offset from the origin the cast should take place</param>
    /// <returns>Returns true if something was hit</returns>
    public static bool Cast(ColliderInfo c, Vector3 castVec, Vector3 offset)
    {
        return Cast(c, castVec, offset, out _);
    }
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <param name="hit">Infromation about the object that was hit</param>
    /// <returns>Returns true if something was hit</returns>
    public static bool Cast(ColliderInfo c, Vector3 castVec, out RaycastHit hit)
    {
        return Cast(c, castVec, Vector3.zero, out hit);
    }
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <param name="offset">The offset from origin the cast should take place</param>
    /// <param name="hit">Information about the object that was hit</param>
    /// <returns>Returns true if something was hit</returns>
    public static bool Cast(ColliderInfo c, Vector3 castVec, Vector3 offset, out RaycastHit hit)
    {   //Perform the raycast and get the results
        RaycastHit[] h = CastAll(c, castVec, offset);
        hit = new RaycastHit();
        //If h hit nothing return false
        if (h == null || h.Length == 0)
            return false;
        //Loop through the results and and find the closest valid result
        for (int i = 0; i < h.Length; i++)
            //If the distance is not 0, return the hit value
            if (h[i].distance != 0)
            {
                hit = h[i];
                return true;
            }
        return false;
    }
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo with their collisionOffset
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <returns>Returns true if something was hit</returns>
    public static bool CastWithOffset(ColliderInfo c, Vector3 castVec)
    {
        return CastWithOffset(c, castVec, Vector3.zero, out _);
    }
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo with collisionOffset
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <param name="offset">The offset from the origin the cast should take place</param>
    public static bool CastWithOffset(ColliderInfo c, Vector3 castVec, Vector3 offset)
    {
        return CastWithOffset(c, castVec, offset, out _);
    }
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo with collisionOffset
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <param name="hit">Infromation about the object that was hit</param>
    /// <returns>Returns true if something was hit</returns>
    public static bool CastWithOffset(ColliderInfo c, Vector3 castVec, out RaycastHit hit)
    {
        return CastWithOffset(c, castVec, Vector3.zero, out hit);
    }
    /// <summary>
    /// Performs a capsual cast using the given colliderInfo with collisionOffset
    /// </summary>
    /// <param name="c">The collider to cast</param>
    /// <param name="castVec">The direction and distance to cast the collider</param>
    /// <param name="offset">The offset from origin the cast should take place</param>
    /// <param name="hit">Information about the object that was hit</param>
    /// <returns>Returns true if something was hit</returns>
    public static bool CastWithOffset(ColliderInfo c, Vector3 castVec, Vector3 offset, out RaycastHit hit)
    {   //Perform the raycast
        RaycastHit[] h = CastAllWithOffset(c, castVec, offset);
        hit = new RaycastHit();
        //If h is null or has a length of 0, return false
        if (h == null || h.Length == 0)
            return false;
        //Loop through the results to find the first valid result and return it
        for (int i = 0; i < h.Length; i++)
            if(h[i].distance != 0)
            {
                hit = h[i];
                return true;
            }
        //Return false if none of the hit results were valid
        return false;
    }
    /// <summary>
    /// Performs a cast and returns infromation about everything hit
    /// </summary>
    /// <param name="c">The colliderInfo to cast</param>
    /// <param name="castVec">The distance and direction of the cast</param>
    /// <returns>Information about everything hit</returns>
    public static RaycastHit[] CastAll(ColliderInfo c, Vector3 castVec)
    {
        return CastAll(c, castVec, Vector3.zero);
    }
    /// <summary>
    /// Performs a cast and return everything that was hit
    /// </summary>
    /// <param name="c">The collider info to cast</param>
    /// <param name="castVec">The direction and distance of the cast</param>
    /// <param name="offset">The positional offset of the cast</param>
    /// <returns>Hit information about everything that was hit</returns>
    public static RaycastHit[] CastAll(ColliderInfo c, Vector3 castVec, Vector3 offset)
    {
        return CastAllGivenRadius(c, castVec, c.Radius, offset);
    }
    /// <summary>
    /// Performs a cast with collisionOffset and returns infromation about everything hit
    /// </summary>
    /// <param name="c">The colliderInfo to cast</param>
    /// <param name="castVec">The distance and direction of the cast</param>
    /// <returns>Information about everything hit</returns>
    public static RaycastHit[] CastAllWithOffset(ColliderInfo c, Vector3 castVec)
    {
        return CastAllWithOffset(c, castVec, Vector3.zero);
    }
    /// <summary>
    /// Performs a cast with collisionOffset and return everything that was hit
    /// </summary>
    /// <param name="c">The collider info to cast</param>
    /// <param name="castVec">The direction and distance of the cast</param>
    /// <param name="offset">The positional offset of the cast</param>
    /// <returns>Hit information about everything that was hit</returns>
    public static RaycastHit[] CastAllWithOffset(ColliderInfo c, Vector3 castVec, Vector3 offset)
    {
        return CastAllGivenRadius(c, castVec, c.Radius + c.CollisionOffset - 1e-5f, offset);
    }
    /// <summary>
    /// Performs the raycast with a given radius. ColliderInfo is used to get the positions
    /// </summary>
    /// <param name="c">The colliderInfo to cast</param>
    /// <param name="castVec">The direction and magnitude of the cast</param>
    /// <param name="radius">The radius of the capsual</param>
    /// <param name="offset">The positional offset of the raycast</param>
    /// <returns>Returns the collision information for the hit objects</returns>
    private static RaycastHit[] CastAllGivenRadius(ColliderInfo c, Vector3 castVec, float radius, Vector3 offset)
    {   //Perform the raycast
        RaycastHit[] h = Physics.CapsuleCastAll(c.GetUpperPoint() + offset, c.GetLowerPoint() + offset, radius, castVec.normalized, castVec.magnitude);
        //Sort the hit info by length so that the closest is first
        System.Array.Sort(h, Conditions.CompareDist);
        return h;
    }
    #endregion
}