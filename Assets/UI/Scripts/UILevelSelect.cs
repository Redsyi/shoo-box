using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manages a level select button
/// </summary>
public class UILevelSelect : MonoBehaviour
{
    public Level level;

    private void Start()
    {
        CheckUnlocked();
    }

    public void CheckUnlocked()
    {
        Button button = GetComponent<Button>();
        Text text = button.GetComponentInChildren<Text>();
        bool editorOverride = false;
#if UNITY_EDITOR
        editorOverride = true;
#endif
        bool enable = PlayerData.unlockedLevels[level.saveID] || editorOverride || Devmode.active;
        button.interactable = enable;
        text.text = !enable ? "Locked" : level.displayName;
    }

    public void LevelSelect()
    {
        PlayerData.currCheckpoint = 0;
        PlayerData.currLevel = level.saveID;
        CheckpointManager.currCheckpoint = 0;
        LevelBridge.BridgeTo(level.cutsceneBuildName, level.cutsceneFlavorText);
    }
}
