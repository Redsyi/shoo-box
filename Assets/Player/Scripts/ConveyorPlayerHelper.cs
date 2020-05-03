using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that manages the final part of the intro conveyor cutscene
/// </summary>
public class ConveyorPlayerHelper : MonoBehaviour
{
    public StealFocusWhenSeen initialFocusStealer;

    void Update()
    {
        if (Player.current.wigglesRequired == 0 && StealFocusWhenSeen.activeThief != null)
        {
            Player.current.lockChangeForm = false;
            initialFocusStealer.Skip();
        }
    }
}
