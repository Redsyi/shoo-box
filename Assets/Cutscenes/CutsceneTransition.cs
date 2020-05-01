using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// deprecated
/// </summary>
public class CutsceneTransition : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public string message;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextLevel()
    {
        StartCoroutine(TransitionWipe(SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1).name));
    }

    IEnumerator TransitionWipe(string level)
    {
        yield return new WaitForSeconds(transitionTime);
        LevelBridge.BridgeTo(level, message);
    }
}
