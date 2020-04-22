using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteObjectiveOnCheckpointLoad : MonoBehaviour, ICheckpointItem
{
    public int[] objectives;

    public void LoadCheckpoint(int checkpointID)
    {
        print("Checkpoint: " + checkpointID);
        if (checkpointID >= 0 && checkpointID < objectives.Length)
        {
            print("Marking off objective: " + objectives[checkpointID]);
            for (int i = 0; i <= checkpointID; i++)
            {
                if (objectives[i] >= 0)
                    ObjectiveTracker.instance.CompleteObjective(objectives[i]);
            }
        }
        else if (checkpointID >= objectives.Length && objectives.Length != 0)
        {
            print("Marking off objective: " + objectives[checkpointID]);
            
            if (objectives[objectives.Length - 1] >= 0)
                ObjectiveTracker.instance.CompleteObjective(objectives[objectives.Length - 1]);
            
        }
    }
}
