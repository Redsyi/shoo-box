using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSoundOnKick : MonoBehaviour, IKickable
{
    public AudioClip clip;
    public float AIRadius;
    public float volume = 1;

    public void OnKick(GameObject kicker)
    {
        AudioManager.MakeNoise(transform.position, AIRadius, clip, volume);
    }
}
