using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// changes the scene after a period of time
/// </summary>
public class ChangeSceneAfterSeconds : MonoBehaviour
{
    public Level destLevel;
    public float afterSeconds;
    public bool isCutscene;

    // Start is called before the first frame update
    void Start()
    {
        if (isCutscene)
        {
            Invoke("ChangeScene", afterSeconds);
        }
    }

    void ChangeScene()
    {
        StartCoroutine(TransitionWipe());
    }

    public void Skip()
    {
        CancelInvoke("ChangeScene");
        ChangeScene();
    }

    public void OnSkip()
    {
        Skip();
    }

    IEnumerator TransitionWipe()
    {
        yield return new WaitForSeconds(1f);
        LevelBridge.BridgeTo(destLevel.levelBuildName, destLevel.levelFlavorText);
    }
}
