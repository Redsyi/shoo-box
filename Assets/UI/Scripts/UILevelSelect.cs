using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages a level select button
/// </summary>
public class UILevelSelect : MonoBehaviour
{
    public Level level;

    public void LevelSelect()
    {
        PlayerData.currCheckpoint = 0;
        PlayerData.currLevel = level.saveID;
        CheckpointManager.currCheckpoint = 0;
        LevelBridge.BridgeTo(level.cutsceneBuildName, level.cutsceneFlavorText);
    }
}
