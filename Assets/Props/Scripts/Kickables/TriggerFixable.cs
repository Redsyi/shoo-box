using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFixable : MonoBehaviour, IAIInteractable
{
    public AIInterest[] interestMask;
    public float interactTime;
    public bool broken;

    public void AIFinishInteract(AIAgent ai)
    {
        broken = true;
    }

    public void AIInteracting(float interactProgress)
    {
    }

    public float AIInteractTime()
    {
        return interactTime;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }

    public bool NeedsInteraction()
    {
        return broken;
    }
}
