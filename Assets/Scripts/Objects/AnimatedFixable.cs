using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedFixable : MonoBehaviour, IAIInteractable
{
    public Animator animator;
    public string breakTriggerName = "Break";
    public string fixTriggerName = "Fix";
    public float fixTime = 3;
    public AIInterest[] interestLayers;
    private bool broken;

    public void AIFinishInteract()
    {
        broken = false;
        animator.SetTrigger(fixTriggerName);
    }

    public void AIInteracting(float interactProgress)
    {
    }

    public float AIInteractTime()
    {
        return fixTime;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestLayers;
    }

    public bool NeedsInteraction()
    {
        return broken;
    }

    public void Break()
    {
        broken = true;
        animator.SetTrigger(breakTriggerName);
    }
}
