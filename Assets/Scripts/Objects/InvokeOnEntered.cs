using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeOnEntered : MonoBehaviour
{
    public UnityEvent invocations;

    private void OnTriggerEnter(Collider other)
    {
        invocations.Invoke();
    }
}
