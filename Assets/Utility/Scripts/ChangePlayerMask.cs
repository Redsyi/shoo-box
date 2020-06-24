using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerMask : MonoBehaviour
{
    public AIInterest[] interestMask;
    public TSAAlert playerTSAALert;
    private AIInterest current;
    private bool leavingFirstArea = true;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(interestMask.Length > 1)
            {
                current = (leavingFirstArea ? interestMask[1] : interestMask[0]);
                leavingFirstArea = !leavingFirstArea;
                if (playerTSAALert.interestMask.Length > 0)
                {
                    playerTSAALert.interestMask[0] = current;
                }
            }
            
        }
    }
}
