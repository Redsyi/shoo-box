using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingJibbitGiver : MonoBehaviour
{
    public float force;
    public CollectableJibbit jibbit;
    public BreakHingeKickable connectedKickable;
    public float delay;

    bool given;

    void Update()
    {
        if (!given && connectedKickable.intactJoints == 0)
        {
            given = true;
            Invoke("SpawnJibbit", delay);
        }
    }

    void SpawnJibbit()
    {
        CollectableJibbit jib = Instantiate(jibbit, transform.position, Quaternion.identity);
        jib.Launch(transform.forward * force);
    }
}
