using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeOnKick : MonoBehaviour, IKickable
{
    public UnityEvent triggers;
    

    public void OnKick(GameObject kicker)
    {
           triggers.Invoke();
    }
}
