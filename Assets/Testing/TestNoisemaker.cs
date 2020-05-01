using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNoisemaker : MonoBehaviour
{
    public float range;
    public AudioClip audioClip;

    public void Noisemake()
    {
        AudioManager.MakeNoise(transform.position, range, audioClip, 1);
    }
}
