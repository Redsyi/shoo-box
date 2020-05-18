using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that is attached to an npc and catches the player
/// </summary>
public class AIPlayerCatcher : MonoBehaviour
{
    public AIAgent agent;
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (agent.state == AIState.CHASE && player != null)
        {
            agent.CatchPlayer(player);
        }
    }
}
