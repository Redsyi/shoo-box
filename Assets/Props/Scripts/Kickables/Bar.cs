using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// holds a bunch of bottles, drops all of them when kicked and alerts a bunch of NPCs
/// </summary>
public class Bar : MonoBehaviour, IKickable
{
    public Rigidbody[] bottles;
    public float force;
    public AIInterest[] interestMask;

    bool alreadyKicked;

    public void OnKick(GameObject kicker)
    {
        if (!alreadyKicked)
        {
            alreadyKicked = true;
            foreach (Rigidbody bottle in bottles)
            {
                bottle.isKinematic = false;
                bottle.AddForce(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized * force);
            }
            AIAgent.SummonAI(Player.current.transform.position, 6, true, interestMask);
        }
    }
}
