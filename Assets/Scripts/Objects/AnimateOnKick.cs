using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnKick : MonoBehaviour, IKickable
{
    public Animator animator;
    public string triggerName;

    public void OnKick(GameObject kicker)
    {
        animator.SetTrigger(triggerName);
    }

    private void Start()
    {
        if (animator == null)
            Debug.LogError("AnimateOnKick not attached to animator");
    }
}
