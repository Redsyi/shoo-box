using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltSwitch : MonoBehaviour, IKickable, ISandalable
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HitBySandal()
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");
    }

    public void OnKick(GameObject kicker)
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");
    }
}
