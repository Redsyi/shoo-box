using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public GameObject currentItem;

    private void OnTriggerEnter(Collider other)
    {
        currentItem = other.gameObject;
        print("item");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentItem)
            currentItem = null;
    }
}
