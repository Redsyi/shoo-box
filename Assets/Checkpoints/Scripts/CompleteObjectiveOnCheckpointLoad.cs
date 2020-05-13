using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that completes objectives when a checkpoint is loaded
/// </summary>
public class CompleteObjectiveOnCheckpointLoad : MonoBehaviour, ICheckpointItem
{
    public int[] objectives;

    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpointID >= 0 && checkpointID < objectives.Length)
        {
            for (int i = 0; i <= checkpointID; i++)
            {
                if (objectives[i] >= 0)
                    ObjectiveTracker.instance.CompleteObjective(objectives[i]);
            }
        }
        else if (checkpointID >= objectives.Length && objectives.Length != 0)
        {
            
            if (objectives[objectives.Length - 1] >= 0)
                ObjectiveTracker.instance.CompleteObjective(objectives[objectives.Length - 1]);
            
        }
    }
}
