using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerCatcher : MonoBehaviour
{
    public AIAgent agent;
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (agent.currState.state == AIState.CHASE && player != null)
        {
            agent.CatchPlayer(player);
        }
    }
}
