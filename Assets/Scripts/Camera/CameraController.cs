using UnityEngine;
/// <summary>
/// Controls the rotation of the camera and player
/// </summary>
public class CameraController : MonoBehaviour
{
#if FREECAM
    /// <summary>
    /// The sensitivity of rotation
    /// </summary>
    [Tooltip("The sensitivity of rotation")]
    [SerializeField]
    private float _sensitivity = 1000;
    /// <summary>
    /// The sensitivity of rotation
    /// </summary>
    public float Sensitivity => _sensitivity;
    /// <summary>
    /// The transform of the players origin or the equivalent location of the character controller
    /// </summary>
    [Tooltip("The transform of the players origin or the equivalent location of the character controller")]
    [SerializeField]
    private Transform _parentTrans = null;
#endif
    /// <summary>
    /// The transform of the head of the model
    /// </summary>
    [Tooltip("The transform of the head of the model")]
    [SerializeField]
    private Transform _trueParent = null;
#if FREECAM
    /// <summary>
    /// The transform of the neck bone. Used to rotate the head to stop the camera seeing the head
    /// </summary>
    [Tooltip("The transform of the neck bone.")]
    [SerializeField]
    private Transform _neckTransform = null;
#endif
    /// <summary>
    /// The vertical offset from the center of the head of the model
    /// </summary>
    [Tooltip("The positional offset from the position of trueParent along the trueParents global rotation")]
    [SerializeField]
    private Vector3 _trueParentOffset = new Vector3();
#if FREECAM
    /// <summary>
    /// The maximum angle the player can look up and down
    /// </summary>
    [Range(0, 90)]
    public float maxAngle = 70;
    /// <summary>
    /// A storage location for the head's angle
    /// </summary>
    private Vector3 headAngle = new Vector3();
#endif
    /// <summary>
    /// Should the camera follow trueParents rotation
    /// </summary>
    [Tooltip("Should the camera follow trueParents rotation")]
    [SerializeField]
    private bool _followHead = false;
    /// <summary>
    /// Should the camera follow trueParents rotation
    /// </summary>
    public bool FollowHead
    {
        get => _followHead;
        set => _followHead = value;
    }
    /// <summary>
    /// Should the camera ignore the trueParents y rotation
    /// </summary>
    [Tooltip("Should the camera ignore the trueParents y rotation")]
    [SerializeField]
    private bool _ignoreYAxisOnHeadFollow = false;
    /// <summary>
    /// Should the camera ignore the trueParents y rotation
    /// </summary>
    public bool IgnoreYAxisOnHeadFollow
    {
        get => _ignoreYAxisOnHeadFollow;
        set => _ignoreYAxisOnHeadFollow = value;
    }

    [SerializeField]
    private float _returnToNormal = 0.5f;

    private float t_returnTime = 0;

    private Vector3 _cameraRotationUponExit = Vector3.zero;
    /// <summary>
    /// Disables the players mouse cursor so they don't see it
    /// </summary>
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    /// <summary>
    /// Updates the players rotation and cameras rotation
    /// </summary>
    void Update()
    {
        if (_trueParent != null)
            transform.position = _trueParent.position + _trueParent.up * _trueParentOffset.y + _trueParent.forward * _trueParentOffset.z + _trueParent.right * _trueParentOffset.x;

        Vector3 angles = transform.eulerAngles;
#if FREECAM
        if (Cursor.lockState == CursorLockMode.Locked)
        {   //Get the mouses movement
            float dx = -Input.GetAxis("Mouse Y");
            float dy = Input.GetAxis("Mouse X");
            //Calculate the change in Y
            dx = angles.x + dx * _sensitivity * Time.deltaTime;

            //Clamp the angle within 70 degrees
            if (dx < 360 && dx < 360 - maxAngle && dx > 180)
                dx = 360 - maxAngle;
            else if (dx >= 0 && dx > maxAngle && dx < 180)
                dx = maxAngle;

            angles.x = dx;
            headAngle = angles;

            Vector3 parentAngle = _parentTrans.eulerAngles;
            parentAngle.y += dy * _sensitivity * Time.deltaTime;
            _parentTrans.eulerAngles = parentAngle;

            angles.y = parentAngle.y;
        }
#endif  //If we are not following the head, don't rotate the camera around the z axis
        if (!_followHead)
            angles.z = 0;
        //Set the camera rotation
        transform.eulerAngles = angles;
        //Toggle the lock state of the camera when the escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }
    /// <summary>
    /// Updates the neck bone of the player so looking directly up won't reveal the players head.
    /// </summary>
    private void LateUpdate()
    {   //If we are following the head, follow the heads rotation
        if (_followHead)
        {   //Ignore the y axis if we want it ignored
            Vector3 angle = _trueParent.eulerAngles;
            if (_ignoreYAxisOnHeadFollow)
                angle.y = 0;
            transform.eulerAngles = angle;
            t_returnTime = 0;
            _cameraRotationUponExit = angle;
        }
#if FREECAM
        else
        {   //Otherwise rotate the head bone to look in the same direction as the camera
            Vector3 ang = _neckTransform.eulerAngles;
            ang.x += headAngle.x;
            _neckTransform.eulerAngles = ang;
        }
#else
        else
        {
            t_returnTime += Time.deltaTime;
            Vector3 angle = transform.eulerAngles;
            //angle = Vector3.Lerp(_cameraRotationUponExit, Vector3.zero, t_returnTime / _returnToNormal);
            transform.eulerAngles = angle;
        }
#endif
    }
    /// <summary>
    /// Resets the rotation of the camera to 0,0,0 in degrees
    /// </summary>
    public void ResetRotation()
    {
        transform.eulerAngles = Vector3.zero;
    }
}
