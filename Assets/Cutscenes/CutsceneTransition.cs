using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        StartCoroutine(TransitionWipe(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator TransitionWipe(int levelIndex)
    {
        yield return new WaitForSeconds(transitionTime);
        LevelBridge.BridgeTo(levelIndex, message);
    }
}
