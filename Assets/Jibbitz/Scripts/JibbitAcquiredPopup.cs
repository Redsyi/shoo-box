using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class that shows the "acquired _____" popup
/// </summary>
public class JibbitAcquiredPopup : MonoBehaviour
{
    /// <summary>
    /// static reference to the current instance of this class
    /// </summary>
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
