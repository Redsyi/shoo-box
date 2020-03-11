using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AKEventOnSandal : MonoBehaviour, ISandalable
{
    public AK.Wwise.Event sandalHitSound;
    public void HitBySandal()
    {
        sandalHitSound.Post(gameObject);
    }
}
