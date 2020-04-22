using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipEnabledOnCheckpointLoad : MonoBehaviour, ICheckpointItem
{
    public MonoBehaviour[][] checkpoints;

    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpointID >= 0 && checkpointID < checkpoints.Length)
        {
            LoadCheckpointSafe(checkpointID);
        }
        else if (checkpointID >= checkpoints.Length && checkpoints.Length != 0)
        {
            LoadCheckpointSafe(checkpoints.Length - 1);
        }
    }

    private void LoadCheckpointSafe(int checkpointID)
    {
        foreach (MonoBehaviour component in checkpoints[checkpointID])
        {
            component.enabled = !component.enabled;
        }
    }
}