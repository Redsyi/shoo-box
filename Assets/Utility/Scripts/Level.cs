using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public string saveID;
    public string cutsceneBuildName;
    public string levelBuildName;
    public string cutsceneFlavorText;
    public string levelFlavorText;
}
