﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneAfterSeconds : MonoBehaviour
{
    public int destinationScene;
    public float afterSeconds;
    public bool isCutscene;
    public string message;

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
        LevelBridge.BridgeTo(destinationScene, message);
    }
}
