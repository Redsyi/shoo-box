using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class UIPopup : MonoBehaviour
{
    private Animator animator;
    private bool active;
    private PlayerInput inputSystem;
    public static bool popupActive => activePopup != null;
    public static UIPopup activePopup;
    public UnityEvent invokeOnDismissed;
    public AK.Wwise.Event onActivation;
    public AK.Wwise.Event onDismissal;

    void Start()
    {
        animator = GetComponent<Animator>();
        inputSystem = FindObjectOfType<PlayerInput>();
    }

    public void Activate()
    {
        if (!active)
        {
            if (animator)
                animator.SetTrigger("Activate");
            inputSystem.SwitchCurrentActionMap("UI");
            activePopup = this;
            active = true;
        }
    }

    public void Dismiss()
    {
        if (active && !UIPauseMenu.instance.paused)
        {
            if (animator)
                animator.SetTrigger("Dismiss");
            inputSystem.SwitchCurrentActionMap("Player");
            activePopup = null;
            active = false;
            invokeOnDismissed.Invoke();
        }
    }

    private void OnDestroy()
    {
        activePopup = null;
    }
}
