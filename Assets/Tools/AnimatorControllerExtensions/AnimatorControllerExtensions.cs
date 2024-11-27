using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;

namespace Dreamscape
{
	///-/////////////////////////////////////////////////////////////////////////////////////
    ///
    public static class AnimatorControllerExtensions
    {
        ///-/////////////////////////////////////////////////////////////////////////////////////
        ///
        /// Returns true if the animation clip is valid in the given state machine in the given animator
        ///
        public static bool ValidateAnimationClip(this AnimatorController controller, string stateMachineName, AnimationClip animationClip)
        {
            // Return false if state machine is null
            AnimatorStateMachine stateMachine = controller.FindSubStateMachine(stateMachineName);
            if (stateMachine == null)
            {
                Debug.Log("Did not find state machine");
                return false;
            }
            Debug.Log("Found state machine");
            
            // Loop through all the states
            foreach (ChildAnimatorState state in stateMachine.states)
            {
                // Check if the state names match. If so, the clip is valid.
                if (state.state.name.Equals(animationClip.name))
                {
                    return true;
                }
            }
            return false;
        }

        ///-/////////////////////////////////////////////////////////////////////////////////////
        ///
        public static AnimatorStateMachine FindSubStateMachine(this AnimatorController controller, string stateMachineName)
        {
            // Iterate through the controller's layers
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                // If found state machine, return it
                AnimatorStateMachine foundSubstateMachine = FindSubStateMachineRecursive(layer.stateMachine, stateMachineName);
                if (foundSubstateMachine != null)
                {
                    return foundSubstateMachine;
                }
            }
            return null;
        }

        ///-/////////////////////////////////////////////////////////////////////////////////////
        ///
        /// Search through the state machines recursively for one of the specified name
        /// 
        public static AnimatorStateMachine FindSubStateMachineRecursive(AnimatorStateMachine stateMachine, string stateMachineName)
        {
            // Iterate through the subStateMachines for the 'Combos' one
            foreach (ChildAnimatorStateMachine subStateMachine in stateMachine.stateMachines)
            {
                // Check current sub stateMachine
                if (subStateMachine.stateMachine.name == stateMachineName)
                {
                    return subStateMachine.stateMachine;
                }
                
                // Recursively search through current sub stateMachine's sub stateMachines
                AnimatorStateMachine foundSubstateMachine = FindSubStateMachineRecursive(subStateMachine.stateMachine, stateMachineName);
                if (foundSubstateMachine != null)
                {
                    return foundSubstateMachine;
                }
            }

            return null;
        }

        ///-/////////////////////////////////////////////////////////////////////////////////////
        ///
        /// Get the animator  from the given Animator
        /// 
        public static AnimatorController GetAnimatorController(Animator animator)
        {
            return animator.runtimeAnimatorController as AnimatorController;
        }
    }

    #endif // UNITY_EDITOR
}
