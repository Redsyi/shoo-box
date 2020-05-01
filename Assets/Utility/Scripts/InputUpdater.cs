using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sets the most recent control scheme when any button is pressed
/// </summary>
public class InputUpdater : MonoBehaviour
{
    UnityEngine.InputSystem.PlayerInput inputSystem;

    private void Awake()
    {
        inputSystem = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    public void OnMove()
    {    
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnLook()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnChangeForm()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnRotate()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnPauseMenu()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnAction()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnTest()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnChangeShoes()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnShoeSight()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnSwap()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnCheckpointLoad()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnCheckpointForceReload()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnNavigate()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnSubmit()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnCancel()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnPoint()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnClick()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnScrollWheel()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnMiddleClick()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnRightClick()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnTutorialContinue()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnChangeTab()
    {
        Controls.UpdateControlScheme(inputSystem);
    }

    public void OnSkip()
    {
        Controls.UpdateControlScheme(inputSystem);
    }
}
