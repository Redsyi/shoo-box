using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeSniffer : MonoBehaviour
{
    public ShoePickup detectedShoe;

    private void OnTriggerEnter(Collider other)
    {
        ShoePickup shoe = other.gameObject.GetComponent<ShoePickup>();
        if (shoe == null)
        {
            Debug.LogWarning("Shoe Sniffer picked up something that's not a shoe pickup!");
        } else
        {
            detectedShoe = shoe;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        detectedShoe = null;
    }
}
