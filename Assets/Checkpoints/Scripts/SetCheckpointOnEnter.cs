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
        checkpointManager.SetCheckpoint(checkpointID);
        gameObject.SetActive(false);
    }
}
