using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that is called whenever a checkpoint is loaded
/// </summary>
public interface ICheckpointItem
{
    void LoadCheckpoint(int checkpointID);
}
