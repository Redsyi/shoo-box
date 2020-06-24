using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Changes NPCs broadcast interest. Really only meant fo terminal walkers but can be modified
 */
public class ChangeNPCMask : MonoBehaviour
{
    [SerializeField] private AIInterest interest = default; // What interest to change to
    [SerializeField] private string targetTag = "Terminal Walker";

    private void OnTriggerEnter(Collider other)
    {

        if(other.transform.parent.CompareTag(targetTag))
        {
            other.GetComponentInParent<AIAgent>().chaseBroadcastInterests[0] = interest; // Assumes there's atleast 1 broadcast interest
        }
    }
}
