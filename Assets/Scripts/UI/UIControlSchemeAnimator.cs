using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControlSchemeAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerInput inputSystem;

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputSystem = FindObjectOfType<PlayerInput>();
    }

    private void Update()
    {
        if (animator)
        {
            animator.SetBool("Controller", inputSystem.currentControlScheme == "Gamepad");
        }
    }
}
