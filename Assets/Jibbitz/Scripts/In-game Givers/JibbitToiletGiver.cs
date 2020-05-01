using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitToiletGiver : MonoBehaviour, IKickable
{
    public float launchVelocity;
    public CollectableJibbit jibbitPrefab;

    bool givenJibbit;

    public void OnKick(GameObject kicker)
    {
        if (!givenJibbit)
        {
            givenJibbit = true;
            CollectableJibbit jib = Instantiate(jibbitPrefab, transform.position, Quaternion.identity);
            jib.Launch(transform.forward * launchVelocity);
        }
    }
}
