using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    static string currControlScheme;
    public static bool usingController => (currControlScheme == "Gamepad");

    public static void UpdateControlScheme(UnityEngine.InputSystem.PlayerInput inputManager)
    {
        currControlScheme = inputManager.currentControlScheme;
    }
}
