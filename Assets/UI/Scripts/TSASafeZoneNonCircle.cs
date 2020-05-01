using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSASafeZoneNonCircle : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        TSAAlert alert = other.GetComponent<TSAAlert>();
        if (alert)
        {
            alert.alertTimeRemaining = alert.alertTime;
        }
    }
}
