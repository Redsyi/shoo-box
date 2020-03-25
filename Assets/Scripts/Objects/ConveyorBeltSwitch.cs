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
        if (ConveyorBelt.active)
        {
            ChangeLevel changeLevel = FindObjectOfType<ChangeLevel>();
            if (changeLevel)
            {
                print("enabling changeLevel");
                changeLevel.canChangeLevels = true;
            }
        }
        else
        {
            ChangeLevel changeLevel = FindObjectOfType<ChangeLevel>();
            if (changeLevel)
            {
                print("Unenabling changeLevel");
                changeLevel.canChangeLevels = false;
            }
        }
    }

    public void OnKick(GameObject kicker)
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");
        if (ConveyorBelt.active)
        {
            ChangeLevel changeLevel = FindObjectOfType<ChangeLevel>();
            if (changeLevel)
            {
                print("enabling changeLevel");
                changeLevel.canChangeLevels = true;
            }
        }
        else
        {
            ChangeLevel changeLevel = FindObjectOfType<ChangeLevel>();
            if (changeLevel)
            {
                print("Unenabling changeLevel");
                changeLevel.canChangeLevels = false;
            }
        }
    }
}
