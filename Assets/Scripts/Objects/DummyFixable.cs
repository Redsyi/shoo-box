using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyFixable : MonoBehaviour, IAIInteractable
{
    public AIInterest[] interestMask;
    public float interactTime;
    public bool broken;

    public void AIFinishInteract()
    {
        broken = false;
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
