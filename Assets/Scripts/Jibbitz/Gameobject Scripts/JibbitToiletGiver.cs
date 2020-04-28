using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitToiletGiver : MonoBehaviour, IKickable
{
    public Color color;
    public float launchVelocity;
    public float size;
    public Jibbit jibbit;
    public CollectableJibbit jibbitPrefab;

    bool givenJibbit;

    public void OnKick(GameObject kicker)
    {
        if (!givenJibbit)
        {
            givenJibbit = true;
            CollectableJibbit jib = Instantiate(jibbitPrefab, transform.position, Quaternion.identity);
            jib.Launch(transform.forward * launchVelocity, color, size, jibbit, 1);
            //JibbitManager.LaunchCollectableJibbit(jibbit, transform.position, transform.forward * launchVelocity, size, color, 1);
        }
    }
}
