﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConveyorBeltSwitch : MonoBehaviour, IKickable, ISandalable, IAIInteractable
{
    private Animator animator;
    public AIInterest[] interestMask;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HitBySandal()
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");

        if (!ConveyorBelt.active)
        {
            SummonOnFling fling = GetComponent<SummonOnFling>();
            if (fling && fling.AIs.Length > 0)
                fling.OnFling();
        }
        /*if (ConveyorBelt.active)
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
            SummonOnFling fling = GetComponent<SummonOnFling>();
            if (fling && fling.AIs.Length > 0)
                fling.OnFling();
            ChangeLevel changeLevel = FindObjectOfType<ChangeLevel>();
            if (changeLevel)
            {
                print("Unenabling changeLevel");
                if(!changeLevel.lockChange)
                    changeLevel.canChangeLevels = false;
            }
        }*/
    }

    public void OnKick(GameObject kicker)
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");
        /*if (ConveyorBelt.active)
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
        }*/
    }

    public float AIInteractTime()
    {
        return .5f;
    }

    public void AIFinishInteract()
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");
       /* if (ConveyorBelt.active)
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
        }*/
    }

    public void AIInteracting(float interactProgress)
    {
        
    }

    public void Toggle()
    {
        ConveyorBelt.ToggleActive();
        animator.SetTrigger("Trigger");
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
