using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// detects when an AI enters a trigger, and sends it to investigate any interactables from a specified list that need interaction.
/// place this object on the "AITriggerable" layer
/// </summary>
public class AIRedirectTrigger : MonoBehaviour
{
    public GameObject[] fixablesToCheck;

    private void OnTriggerEnter(Collider other)
    {
        AIAgent ai = other.GetComponentInParent<AIAgent>();
        if (ai)
        {
            foreach (GameObject fixable in fixablesToCheck)
            {
                if (fixable)
                {
                    IAIInteractable interactable = fixable.GetComponentInChildren<IAIInteractable>();
                    if (interactable.NeedsInteraction())
                    {
                        ai.Investigate((interactable as MonoBehaviour).gameObject, forceOverrideInteract: false);
                    }
                    //break;
                }
            }
        }
    }
}
