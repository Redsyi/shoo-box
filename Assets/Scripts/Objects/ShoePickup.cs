using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoePickup : MonoBehaviour
{
    public ShoeType shoeType;
    public AK.Wwise.Event shoePickUp;

    private void OnDestroy()
    {
        shoePickUp.Post(gameObject);
        if (shoeType == ShoeType.BOOTS)
        {
            print("Picked up boots");
            ObjectiveTracker.instance.CompleteObjective(0); // Completes first objective in tutorial
        } 
    }
}
