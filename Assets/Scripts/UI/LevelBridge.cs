using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelBridge : MonoBehaviour
{
    public static string destLevel;
    private float timeLeft = 1.5f;
    public static string message;
    public Text messageText;
    private static bool bridging;
    public Animator transitionAnimator;
    private static int prevLevel;
    public Camera backupCam;

    void Start()
    {
        messageText.text = message;
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(1);
        AsyncOperation prevUnloadOperation = SceneManager.UnloadSceneAsync(prevLevel);
        Time.timeScale = 0;
        while (!prevUnloadOperation.isDone)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
        }
        backupCam.enabled = true;
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync(destLevel, LoadSceneMode.Additive);
        while (!levelLoadOperation.isDone)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
        }
        backupCam.enabled = false;
        while (timeLeft > 0f)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(destLevel));
        Time.timeScale = 1;
        transitionAnimator.SetTrigger("End");
        bridging = false;
        yield return new WaitForSeconds(0.9f);
        SceneManager.UnloadSceneAsync("LevelBridge");
    }

    public static void BridgeTo(string level, string msg, bool resetCheckpoints = true)
    {
        if (!bridging)
        {
            prevLevel = SceneManager.GetActiveScene().buildIndex;
            bridging = true;
            destLevel = level;
            message = msg;
            SceneManager.LoadScene("LevelBridge", LoadSceneMode.Additive);
            if (resetCheckpoints)
            {
                CheckpointManager.currCheckpoint = 0;
                CheckpointTransformSaver.ResetAll();
            }
        }
    }

    public static void Reload(string msg)
    {
        BridgeTo(SceneManager.GetActiveScene().name, msg, false);
    }
}
