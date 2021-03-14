using UnityEngine;
/// <summary>
/// Used when the player is selecting their character model
/// </summary>
[RequireComponent(typeof(Animator))]
public class AISelector : MonoBehaviour
{
    /// <summary>
    /// A reference to the animator on this object
    /// </summary>
    private Animator _animator = null;
    /// <summary>
    /// The information about this model
    /// </summary>
    [Tooltip("The information about this model")]
    [SerializeField]
    private ModelInfo m_modelInfo;
    /// <summary>
    /// The information about this object
    /// </summary>
    public ModelInfo ModelInfo => m_modelInfo;
    /// <summary>
    /// The offset from the inital spawn location applied to this objects transform until the AI has loaded its animations. This is in global co-ordinates
    /// </summary>
    [Tooltip("The offset from the inital spawn location applied to this objects transform until the AI has loaded its animations. This is in global co-ordinates")]
    [SerializeField]
    private Vector3 _spawnOffset = new Vector3();
    /// <summary>
    /// Stores wether the teleport has accured
    /// </summary>
    private bool teleported = false;
    /// <summary>
    /// Gets references and sets the position of the NPC
    /// </summary>
    void Start()
    {
        _animator = GetComponent<Animator>();
        transform.position += _spawnOffset;
    }
    /// <summary>
    /// Checks if the AI has entered the animation and teleports them
    /// </summary>
    void Update()
    {
        if (!teleported && _animator.GetBool("InAnimation"))
        {
            teleported = true;
            transform.position -= _spawnOffset;
        }
    }
    /// <summary>
    /// Hook the ModelInfo up to the model info of a prefab. 
    /// This is used after instantiation of a model to keep reference to prefab for when the model is deleted to avoid null variables being set
    /// </summary>
    /// <param name="prefab">The prefab containing the ModelInfo on a AISelector gameObject</param>
    public void SetModelInfoToPrefab(GameObject prefab)
    {
        m_modelInfo = prefab.GetComponentInChildren<AISelector>().ModelInfo;
    }
}
