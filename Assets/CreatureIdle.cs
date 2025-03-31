using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureIdle : StateMachineBehaviour
{
    public int maxLoops = 3;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime > maxLoops)
        {
            animator.SetBool("isLooking", true);
        }
        else
        {
            animator.SetBool("isLooking", false);
        }
    }
}
