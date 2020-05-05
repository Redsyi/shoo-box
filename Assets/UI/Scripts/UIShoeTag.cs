﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Event = AK.Wwise.Event;

/// <summary>
/// manages the "current shoe tag" UI element
/// </summary>
public class UIShoeTag : MonoBehaviour
{
    public Image activeTag;
    public Image inactiveTag;
    public Animator tagAnimator;

    public Sprite noShoeSprite;
    public Sprite bootSprite;
    public Sprite sandalSprite;

    public Event swapSound;
    public Event noSwapSound;

    public void SwitchTo(ShoeType shoe)
    {
        swapSound.Post(gameObject);
        switch(shoe)
        {
            case ShoeType.BOOTS:
                inactiveTag.sprite = bootSprite;
                break;
            case ShoeType.FLIPFLOPS:
                inactiveTag.sprite = sandalSprite;
                break;
            default:
                Debug.LogWarning("Shoe Tag UI switching to unsupported shoe type");
                tagAnimator.SetTrigger("CantSwitch");
                return;
        }
        tagAnimator.SetTrigger("Switch");
    }

    public void Wiggle()
    {
        noSwapSound.Post(gameObject);
        tagAnimator.SetTrigger("CantSwitch");
    }

    public void FinishedSwitchTags()
    {
        activeTag.sprite = inactiveTag.sprite;
    }
}