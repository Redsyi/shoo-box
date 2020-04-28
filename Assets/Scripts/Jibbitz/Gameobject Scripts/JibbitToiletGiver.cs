using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitToiletGiver : MonoBehaviour, IKickable
{
    public Color color;
    public float launchVelocity;
    public float size;
    public Jibbit jibbit;

    bool givenJibbit;

    public void OnKick(GameObject kicker)
    {
        if (!givenJibbit)
        {
            givenJibbit = true;
            JibbitManager.LaunchCollectableJibbit(jibbit, transform.position, transform.forward * launchVelocity, size, color, 1);
        }
    }
}
