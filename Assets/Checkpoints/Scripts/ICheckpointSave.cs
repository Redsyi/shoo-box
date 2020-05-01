using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interface that is called whenever a checkpoint is saved
/// </summary>
public interface ICheckpointSave
{
    void SaveCheckpoint(int checkpointID);
}
