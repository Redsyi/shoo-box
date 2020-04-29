using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitStarsGiver : MonoBehaviour
{
    public CollectableJibbit jibbit;
    public AIAgent attachedAgent;
    public float force;

    bool given;

    void Update()
    {
        if (!given && attachedAgent.stunned)
        {
            given = true;
            Vector3 forceVect = (Player.current.transform.position - attachedAgent.transform.position).normalized + Vector3.up;
            CollectableJibbit jib = Instantiate(jibbit, transform.position, Quaternion.identity);
            jib.Launch(forceVect * force);
        }
    }
}
