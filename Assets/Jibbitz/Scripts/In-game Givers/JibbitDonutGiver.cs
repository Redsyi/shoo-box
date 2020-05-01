using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitDonutGiver : MonoBehaviour
{
    public CollectableJibbit jibbit;
    public float force;
    public Transform launcher;

    public static bool given;

    void Start()
    {
        given = false;
    }

    public void HitByDonut()
    {
        if (!given)
        {
            CollectableJibbit jib = Instantiate(jibbit, launcher.position, Quaternion.identity);
            jib.Launch(launcher.forward * force);
        }
    }
}
