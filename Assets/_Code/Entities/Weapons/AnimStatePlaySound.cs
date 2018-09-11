using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStatePlaySound : StateMachineBehaviour
{
    public string soundName;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.Play(soundName);
    }
}
