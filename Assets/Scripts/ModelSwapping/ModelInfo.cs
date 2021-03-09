using UnityEngine;
/// <summary>
/// Stores information about the model
/// </summary>

[System.Serializable]
public struct ModelInfo
{
    /// <summary>
    /// The avatar of the model
    /// </summary>
    [SerializeField]
    [Tooltip("The avatar of the model")]
    private Avatar _modelAvatar;
    /// <summary>
    /// The avatar of the model
    /// </summary>
    public Avatar ModelAvatar => _modelAvatar;
    /// <summary>
    /// A reference to the model with its skeleton
    /// </summary>
    [SerializeField]
    [Tooltip("A reference to the model with its skeleton")]
    private GameObject _model;
    /// <summary>
    /// A reference to the model with its skeleton
    /// </summary>
    public GameObject Model => _model;
    /// <summary>
    /// The root bone of the avatars skeleton
    /// </summary>
    [SerializeField]
    [Tooltip("The root bone of the avatar. This is only necessary for the players default model")]
    private Transform _rootBone;
    /// <summary>
    /// The root bone of the avatars skeleton
    /// </summary>
    public Transform RootBone => _rootBone;
    /// <summary>
    /// Returns true if everything has been set to valid values
    /// </summary>
    public bool IsValid
    {
        get
        {
            return _modelAvatar != null && _model != null;
        }
    }
    /// <summary>
    /// Clears the struct, setting all values to null
    /// </summary>
    public void Clear()
    {
        _model = null;
        _modelAvatar = null;
        _rootBone = null;
    }
}
