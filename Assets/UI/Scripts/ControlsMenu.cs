using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] GameObject keyboardControls;
    [SerializeField] GameObject gamepadControls;

    public void ControlsPaneLoaded()
    {
        bool usingGamepad = Controls.usingController;
        gamepadControls.SetActive(usingGamepad);
        keyboardControls.SetActive(!usingGamepad);
    }
}
