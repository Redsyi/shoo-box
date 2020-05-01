using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that plays a Wwise event when this object is hit by a sandal
/// </summary>
public class AKEventOnSandal : MonoBehaviour, ISandalable
{
    public AK.Wwise.Event sandalHitSound;
    public void HitBySandal()
    {
        sandalHitSound.Post(gameObject);
    }
}
