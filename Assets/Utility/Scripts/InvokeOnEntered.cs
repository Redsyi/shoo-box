using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// invokes a unityevent when something enters this trigger
/// </summary>
public class InvokeOnEntered : MonoBehaviour
{
    public UnityEvent invocations;

    private void OnTriggerEnter(Collider other)
    {
        invocations.Invoke();
    }
}
