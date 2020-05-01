using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages a level select button
/// </summary>
public class UILevelSelect : MonoBehaviour
{
    public string buildName;
    public string levelName;

    public void LevelSelect()
    {
        LevelBridge.BridgeTo(buildName, levelName);
    }
}
