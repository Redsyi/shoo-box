using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerCatcher : MonoBehaviour
{
    public AIAgent agent;
    private void OnTriggerEnter(Collider other)
    {
        print("collider enter");
        Player player = other.GetComponentInParent<Player>();
        if (agent.currState.state == AIState.CHASE && player != null)
        {
            print("PLAYER");
            agent.CatchPlayer(player);
        }
    }
}
