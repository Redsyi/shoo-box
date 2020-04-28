using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitTVGiver : MonoBehaviour
{
    public Color color;
    public float launchVelocity;
    public float size;
    public Jibbit jibbit;
    public float stareTimeRequired;
    public CollectableJibbit jibbitPrefab;

    bool givenJibbit;
    float timeToJibbit;

    private void OnTriggerStay(Collider other)
    {
        if (!givenJibbit && timeToJibbit < stareTimeRequired)
        {
            if (other.transform.position.z < transform.position.z)
            {
                timeToJibbit += Time.deltaTime;
                if (timeToJibbit >= stareTimeRequired)
                {
                    CollectableJibbit jib = Instantiate(jibbitPrefab, transform.position, Quaternion.identity);
                    jib.Launch(transform.forward * launchVelocity, color, size, jibbit, 1);
                    //JibbitManager.LaunchCollectableJibbit(jibbit, transform.position, transform.forward * launchVelocity, size, color, 1);
                    givenJibbit = true;
                }
            } else
            {
                timeToJibbit = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        timeToJibbit = 0;
    }
}
