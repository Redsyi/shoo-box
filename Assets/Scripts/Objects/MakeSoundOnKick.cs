using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSoundOnKick : MonoBehaviour, IKickable
{
    public float AIRadius;

    public void OnKick(GameObject kicker)
    {
        AudioManager.MakeNoise(transform.position, AIRadius, null, 1);
    }
}
