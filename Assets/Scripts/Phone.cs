using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour, IKickable
{
    public GameObject door;
    public AudioClip clip;

    public void OnKick(GameObject kicker)
    {
        FindObjectOfType<Maid>().Investigate(door);
        AudioManager.MakeNoise(transform.position, 3, clip, 1);
    }
}
