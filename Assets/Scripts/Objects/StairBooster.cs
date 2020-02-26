using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBooster : MonoBehaviour
{
    public float boost;
    private void OnTriggerStay(Collider other)
    {
        other.GetComponentInParent<Player>().verticalBoost = boost;
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponentInParent<Player>().verticalBoost = 0f;
    }
}
