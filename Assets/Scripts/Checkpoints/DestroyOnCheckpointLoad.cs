using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
