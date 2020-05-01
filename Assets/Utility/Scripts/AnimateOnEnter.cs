using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// triggers an animation when a trigger is entered
/// </summary>
public class AnimateOnEnter : MonoBehaviour
{
    public Animator animator;
    public string triggerName;

    public void OnTriggerEnter(Collider other)
    {
        print("Should be animating");
        animator.SetTrigger(triggerName);
    }

    private void Start()
    {
        if (animator == null)
            Debug.LogError("AnimateOnEnter not attached to animator");
    }
}
