using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// changes the level when a trigger is entered, with an optional delay
/// </summary>
public class ChangeLevel : MonoBehaviour
{
    public Level destLevel;
    public bool canChangeLevels = true;
    public bool lockChange;
    public float delay;
    public bool saveProgress = true;

    public void OnTriggerEnter(Collider other)
    {
        if (canChangeLevels && other.gameObject.CompareTag("Player"))
        {
            canChangeLevels = false;
            if (delay == 0f)
                DoChangeLevel();
            else
                Invoke("DoChangeLevel", delay);
        }
    }

    void DoChangeLevel()
    {
        if (saveProgress)
        {
            PlayerData.currLevel = destLevel.saveID;
            PlayerData.currCheckpoint = 0;
            PlayerData.UnlockLevel(destLevel);
        }
        CheckpointManager.currCheckpoint = 0;
        LevelBridge.BridgeTo(destLevel.cutsceneBuildName, destLevel.cutsceneFlavorText);
    }

    public void SetCanChangeLevel(bool value)
    {
        canChangeLevels = value;
    }
}
