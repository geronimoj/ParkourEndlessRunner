using UnityEngine;
/// <summary>
/// Determines when the AI has entered the animation. This is used to avoid T-pose
/// </summary>
public class Entered : StateMachineBehaviour
{   
    /// <summary>
    /// Sets a boolean named InAnimation to true once the animator has entered the state
    /// </summary>
    /// <param name="animator">A reference to the animator</param>
    /// <param name="stateInfo">Information about the current state</param>
    /// <param name="layerIndex">The layer the animator is in</param>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("InAnimation", true);   
    }
}
