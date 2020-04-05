using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
