using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitGhostTracker : MonoBehaviour
{
    public AIAgent disqualifier;
    public bool resetProgress;
    public bool spawnIfQualified;
    public CollectableJibbit jibbit;
    public bool markPlayedTutorial;
    public bool markPlayedLobby;

    static bool qualified;
    static bool playedTutorial;
    static bool playedLobby;

    private void Start()
    {
        if (resetProgress)
        {
            qualified = true;
            playedTutorial = false;
            playedLobby = false;
        }

        if (markPlayedLobby)
            playedLobby = true;

        if (markPlayedTutorial)
            playedTutorial = true;
        

        if (spawnIfQualified && qualified && playedTutorial && playedLobby)
        {
            CollectableJibbit jib = Instantiate(jibbit, transform.position, Quaternion.identity, transform);
            jib.Launch(Vector3.up * 200);
        }
    }

    private void LateUpdate()
    {
        if (qualified && disqualifier && disqualifier.currState.state == AIState.CHASE)
        {
            qualified = false;
        }
    }
}
