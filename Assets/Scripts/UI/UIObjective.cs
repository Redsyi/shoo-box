using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIObjective : MonoBehaviour
{
    public Animator animator;
    public Text text;
    public void Complete()
    {
        animator.SetTrigger("Complete");
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
