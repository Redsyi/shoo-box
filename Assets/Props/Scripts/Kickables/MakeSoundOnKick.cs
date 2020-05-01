using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// makes a noise audible to npcs on kick
/// </summary>
public class MakeSoundOnKick : MonoBehaviour, IKickable
{
    public float AIRadius;

    public void OnKick(GameObject kicker)
    {
        AudioManager.MakeNoise(transform.position, AIRadius, null, 1);
    }
}
