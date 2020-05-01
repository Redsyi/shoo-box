using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that warps a pathfinder on checkpoint loads. Use this class instead of SetPositionOnCheckpoint whenever a NavMeshAgent is involved.
/// </summary>
public class WarpOnCheckpoint : MonoBehaviour, ICheckpointItem
{
    public UnityEngine.AI.NavMeshAgent pathfinder;
    public Vector3[] positions;
    
    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpointID >= 0 && checkpointID < positions.Length)
        {
            pathfinder.Warp(positions[checkpointID]);
        } else if (checkpointID >= positions.Length && positions.Length != 0)
        {
            pathfinder.Warp(positions[positions.Length - 1]);
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
