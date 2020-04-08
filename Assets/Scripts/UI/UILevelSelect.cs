using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelect : MonoBehaviour
{
    public int levelIndex;
    public string levelName;

    public void LevelSelect()
    {
        LevelBridge.BridgeTo(levelIndex, levelName);
    }
}
