using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JibbitAcquiredPopup : MonoBehaviour
{
    public static JibbitAcquiredPopup current;
    public Animator animator;
    public Text text;

    void Start()
    {
        current = this;
    }

    public void Acquire(Jibbit jibbit)
    {
        text.text = $"Collectable acquired: {jibbit.displayName}!";
        animator.SetTrigger("Collect");
    }
}
