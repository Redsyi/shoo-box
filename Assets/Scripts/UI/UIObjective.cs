using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIObjective : MonoBehaviour
{
    public Animator animator;
    public Text text;
    private bool complete;
    public AK.Wwise.Event onCheck;


    public void Complete()
    {
        if (!complete)
        {
            if (animator)
                animator.SetTrigger("Complete");
            complete = true;
            if (this && gameObject)
                onCheck.Post(gameObject);
        }
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
