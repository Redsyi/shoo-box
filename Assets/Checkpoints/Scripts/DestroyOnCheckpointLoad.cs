using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that destroys the object if a certain checkpoint or later is loaded.
/// </summary>
public class DestroyOnCheckpointLoad : MonoBehaviour, ICheckpointItem
{
    public int checkpoint;

    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpoint <= checkpointID)
        {
            Destroy(gameObject);
        }
    }
}
