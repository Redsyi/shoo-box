using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                if (fixable.GetComponent<IAIInteractable>().NeedsInteraction())
                {
                    ai.Investigate(fixable.gameObject);
                    break;
                }
            }
        }
    }
}
