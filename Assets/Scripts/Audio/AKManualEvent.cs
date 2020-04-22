using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AKManualEvent : MonoBehaviour
{
    public AK.Wwise.Event clip;
    
    public void Trigger()
    {
        clip.Post(gameObject);
    }
}
