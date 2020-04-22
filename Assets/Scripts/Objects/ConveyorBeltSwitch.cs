using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConveyorBeltSwitch : MonoBehaviour, IKickable, ISandalable, IAIInteractable
{
    private Animator animator;
    public AIInterest[] interestMask;
    public AK.Wwise.Event sound;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HitBySandal()
    {
        Toggle();

        if (!ConveyorBelt.active)
        {
            SummonOnFling fling = GetComponent<SummonOnFling>();
            if (fling && fling.AIs.Length > 0)
                fling.OnFling();
        }
    }

    public void OnKick(GameObject kicker)
    {
        Toggle();
    }

    public float AIInteractTime()
    {
        return .5f;
    }

    public void AIFinishInteract()
    {
        Toggle();
    }

    public void AIInteracting(float interactProgress)
    {
        
    }

    public void Toggle()
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");
        sound.Post(gameObject);
    }

    public void ToggleAfterTime(float time)
    {
        Invoke("Toggle", time);
    }

    public bool NeedsInteraction()
    {
        return !ConveyorBelt.active;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }
}
