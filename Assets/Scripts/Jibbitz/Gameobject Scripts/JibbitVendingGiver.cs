using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitVendingGiver : MonoBehaviour, IKickable
{
    public int kicksRequired;
    public CollectableJibbit jibbit;
    public float spawnForce;
    public Transform spawner;

    int timesKicked;

    public void OnKick(GameObject kicker)
    {
        if (timesKicked < kicksRequired)
        {
            ++timesKicked;
            if (timesKicked == kicksRequired)
            {
                CollectableJibbit item = Instantiate(jibbit, spawner.position, Quaternion.identity);
                item.Launch(spawner.forward * spawnForce);
            }
        }
    }
}
