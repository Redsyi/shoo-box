using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// singleton class that keeps track of the current control scheme
/// </summary>
public class Controls : MonoBehaviour
{
    static string currControlScheme;
    public static bool usingController => (currControlScheme == "Gamepad");

    public static void UpdateControlScheme(UnityEngine.InputSystem.PlayerInput inputManager)
    {
        currControlScheme = inputManager.currentControlScheme;
    }
}
