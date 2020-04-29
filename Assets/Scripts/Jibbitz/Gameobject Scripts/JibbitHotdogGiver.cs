using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitHotdogGiver : MonoBehaviour
{
    public CollectableJibbit jibbit;
    public Transform launcher;

    static bool[] hits;
    public static bool given;
    const float force = 500;

    private void Start()
    {
        hits = new bool[3];
        given = false;
    }

    public void NotifyHit(int hit)
    {
        hits[hit] = true;
        if (!given && System.Array.TrueForAll(hits, h => h))
        {
            given = true;
            CollectableJibbit jib = Instantiate(jibbit, launcher.position, Quaternion.identity);
            jib.Launch(launcher.forward * force);
        }
    }
}
