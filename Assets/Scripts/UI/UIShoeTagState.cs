using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShoeTagState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (animatorStateInfo.IsName("SwitchTags"))
        {
            FindObjectOfType<UIShoeTag>().FinishedSwitchTags();
        }
    }
}
