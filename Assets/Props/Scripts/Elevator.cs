using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Animator animator;

    public void Open()
    {
        animator.SetTrigger("Open");
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }
}
