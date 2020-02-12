using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShoeTag : MonoBehaviour
{
    public Image activeTag;
    public Image inactiveTag;
    public Animator tagAnimator;

    public Sprite noShoeSprite;
    public Sprite bootSprite;

    public void SwitchTo(ShoeType shoe)
    {
        switch(shoe)
        {
            case ShoeType.BOOTS:
                inactiveTag.sprite = bootSprite;
                break;
            default:
                tagAnimator.SetTrigger("CantSwitch");
                return;
        }
        tagAnimator.SetTrigger("Switch");
    }

    public void FinishedSwitchTags()
    {
        activeTag.sprite = inactiveTag.sprite;
    }
}
