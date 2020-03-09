using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoePickup : MonoBehaviour
{
    public ShoeType shoeType;

    private void OnDestroy()
    {
        if (shoeType == ShoeType.BOOTS)
        {
            ObjectiveTracker.instance.CompleteObjective(0);
        }
    }
}
