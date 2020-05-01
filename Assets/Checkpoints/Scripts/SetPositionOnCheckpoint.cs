using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that teleports this object on a checkpoint load.
/// </summary>
public class SetPositionOnCheckpoint : MonoBehaviour, ICheckpointItem
{
    public Vector3[] positions;
    
    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpointID >= 0 && checkpointID < positions.Length)
        {
            transform.position = positions[checkpointID];
        } else if (checkpointID >= positions.Length && positions.Length != 0)
        {
            transform.position = positions[positions.Length - 1];
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < positions.Length; ++i)
        {
            Gizmos.color = CheckpointManager.checkpointColors[i % CheckpointManager.checkpointColors.Length];
            Gizmos.DrawSphere(positions[i], 0.5f);
        }
    }
}
