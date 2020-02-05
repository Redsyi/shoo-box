using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour, IKickable
{
    public GameObject door;
    public AudioClip clip;

    public void OnKick(GameObject kicker)
    {
        AudioManager.MakeNoise(door.transform.position, 6, clip, 1);
        //FindObjectOfType<Maid>().Investigate(door);
    }
}
