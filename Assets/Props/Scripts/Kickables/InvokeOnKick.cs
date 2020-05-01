using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//invokes a UnityEvent when kicked
public class InvokeOnKick : MonoBehaviour, IKickable
{
    public UnityEvent triggers;
    

    public void OnKick(GameObject kicker)
    {
           triggers.Invoke();
    }
}
