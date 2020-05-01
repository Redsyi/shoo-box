using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// helper to decide which luggagecart component to activate
/// </summary>
public class LuggageCartManager : MonoBehaviour, IKickable
{ 
    public LuggageCart fromLeft; // Left trigger
    public LuggageCart fromRight; // Right trigger
    public BoxCollider leftTrigger;
    public BoxCollider rightTrigger;
    private bool kickedFromLeft = true; // True if kicked from the left side

    public void OnKick(GameObject kicker)
    {
            // If kickedFromLeft == true, then calls the leftTrigger's OnKick(). Vice versa
            (kickedFromLeft ? fromLeft : fromRight).OnKick(kicker);
            print("kickedFromLeft: " + kickedFromLeft);
            kickedFromLeft = !kickedFromLeft;
            leftTrigger.enabled = kickedFromLeft; // If kicked from left, turn off this object. Vice versa
            rightTrigger.enabled = !kickedFromLeft; // Similar to previous line but from the right
    }
}
