using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the transition between levels. This is accomplished with the help of the "LevelBridge" segue scene
/// </summary>
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

    /// <summary>
    /// handles the actual transition sequence. at the point this starts, the levelbridge scene has already been loaded
    /// </summary>
    IEnumerator LoadLevel()
    {
        yield return new WaitForSeconds(1);

        //starts an operation to unload the previous scene
        AsyncOperation prevUnloadOperation = SceneManager.UnloadSceneAsync(prevLevel);

        //pauses game so future scene doesn't process anything before we want it to
        Time.timeScale = 0;

        //wait for scene to be unloaded
        while (!prevUnloadOperation.isDone)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
        }

        //currently no scenes have active camera, so activate ours
        backupCam.enabled = true;

        //now, start to load the destination scene
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync(destLevel, LoadSceneMode.Additive);

        //wait for it to finish
        while (!levelLoadOperation.isDone)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
        }

        //now that it's done, we can turn off our camera
        backupCam.enabled = false;

        //we want to make sure the player has time to read the text between levels, so we enforce a minimum delay between levels
        while (timeLeft > 0f)
        {
            yield return null;
            timeLeft -= Time.unscaledDeltaTime;
        }

        //set destination scene as the active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(destLevel));

        //unpause
        Time.timeScale = 1;

        //swipe the transition off-screen to the left
        transitionAnimator.SetTrigger("End");
        bridging = false;

        //give time for the animation to finish, then unload the bridge scene
        yield return new WaitForSeconds(0.9f);
        SceneManager.UnloadSceneAsync("LevelBridge");
    }

    /// <summary>
    /// bridge to the specified level, showing a message
    /// </summary>
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

    /// <summary>
    /// reloads the current scene
    /// </summary>
    public static void Reload(string msg)
    {
        BridgeTo(SceneManager.GetActiveScene().name, msg, false);
    }
}
