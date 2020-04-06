using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteObjectiveOnCheckpointLoad : MonoBehaviour, ICheckpointItem
{
    public int[] objectives;

    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpointID >= 0 && checkpointID < objectives.Length)
        {
            if (objectives[checkpointID] >= 0)
                ObjectiveTracker.instance.CompleteObjective(objectives[checkpointID]);
        }
        else if (checkpointID > objectives.Length && objectives.Length != 0)
        {
            if (objectives[objectives.Length - 1] >= 0)
                ObjectiveTracker.instance.CompleteObjective(objectives[objectives.Length - 1]);
        }
    }
}
