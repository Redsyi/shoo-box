using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to manage a shoe that can be picked up by the player
/// </summary>
public class ShoePickup : MonoBehaviour
{
    public ShoeType shoeType;
    public AK.Wwise.Event shoePickUp;

    private void OnDestroy()
    {
        shoePickUp.Post(gameObject);
        if (shoeType == ShoeType.BOOTS)
        {
            ObjectiveTracker.instance.CompleteObjective(0); // Completes first objective in tutorial
        } 
    }
}
