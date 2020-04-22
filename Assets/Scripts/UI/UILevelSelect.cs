using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelect : MonoBehaviour
{
    public string buildName;
    public string levelName;

    public void LevelSelect()
    {
        LevelBridge.BridgeTo(buildName, levelName);
    }
}
