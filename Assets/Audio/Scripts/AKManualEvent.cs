using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that plays a wwise event when Trigger() is called
/// </summary>
public class AKManualEvent : MonoBehaviour
{
    public AK.Wwise.Event clip;
    
    public void Trigger()
    {
        clip.Post(gameObject);
    }
}
