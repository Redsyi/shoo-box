using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeOnCheckpointLoad : MonoBehaviour, ICheckpointItem
{
    public UnityEvent[] invocations;


    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpointID >= 0 && checkpointID < invocations.Length)
        {
            LoadCheckpointSafe(checkpointID);
        }
        else if (checkpointID > invocations.Length && invocations.Length != 0)
        {
            LoadCheckpointSafe(invocations.Length - 1);
        }
    }

    private void LoadCheckpointSafe(int checkpointID)
    {
        invocations[checkpointID].Invoke();
    }
}
