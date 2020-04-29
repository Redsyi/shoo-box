using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitWineGiver : MonoBehaviour, IKickable
{
    public CollectableJibbit jibbit;
    public float force;
    public float delay;
    public Transform launcher;

    bool given;

    public void OnKick(GameObject kicker)
    {
        if (!given)
        {
            given = true;
            Invoke("LaunchJibbit", delay);
        }
    }

    void LaunchJibbit()
    {
        CollectableJibbit jib = Instantiate(jibbit, launcher.position, Quaternion.identity);
        jib.Launch(launcher.forward * force);
    }
}
