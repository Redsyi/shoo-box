using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that sets a checkpoint when something enters its trigger. Put the object on the PlayerCatcher layer to only allow the player to trigger it.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class SetCheckpointOnEnter : MonoBehaviour
{
    CheckpointManager checkpointManager;
    public int checkpointID;
    void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = FindObjectOfType<Player>();
        if (player && player.npcsChasing < 1) // Only set checkpoint if player isn't being chased
        {
            // Grab the checkpoint message object. Should be the only object where tag == Checkpoint Message
            Animator message = GameObject.FindGameObjectWithTag("Checkpoint Message").GetComponent<Animator>();
            if (message)
            {
                message.SetTrigger("Checkpoint Hit");
            }
            else
                Debug.LogError("Checkpoint message object not found");
            checkpointManager.SetCheckpoint(checkpointID);
            gameObject.SetActive(false);
        }
    }
}
