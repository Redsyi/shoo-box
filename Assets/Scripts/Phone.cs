using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour, IKickable
{
    public GameObject door;

    public void OnKick(GameObject kicker)
    {
        FindObjectOfType<Maid>().Investigate(door);
    }
}
