using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformResetFixable : MonoBehaviour, IKickable, IAIInteractable
{
    private bool broken;
    public float interactTime;
    public AIInterest[] interestMask;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }
    public void AIFinishInteract()
    {
        broken = false;
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
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

    public void OnKick(GameObject kicker)
    {
        broken = true;
    }
}
