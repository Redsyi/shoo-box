using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;

/// <summary>
/// class that manages the checkpoint system
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    private static int currLevel = -1;
    public static int currCheckpoint = 0;

    //colors used to draw gizmos
    public static Color[] checkpointColors = { Color.blue, Color.red, Color.green, Color.cyan };

    void Start()
    {
        //checks if we are loading in a new level, if so, reset checkpoint. for loop required otherwise it will see the level bridge and always assume new level.
        //deprecated - now ChangeLevel manages resetting the checkpoint in a much cleaner way
        /*for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "LevelBridge" && scene.buildIndex != currLevel)
            {
                currLevel = scene.buildIndex;
                currCheckpoint = 0;
                break;
            }
        }*/
        StartCoroutine(ReloadDelayed());
    }

    /// <summary>
    /// calls ReloadCheckpointItems after one frame
    /// </summary>
    IEnumerator ReloadDelayed()
    {
        yield return null;
        ReloadCheckpointItems();
    }

    /// <summary>
    /// calls LoadCheckpoint on every ICheckpointItem
    /// </summary>
    public void ReloadCheckpointItems()
    {
        foreach (ICheckpointItem checkpointItem in FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointItem>())
        {
            checkpointItem.LoadCheckpoint(currCheckpoint);
        }
    }

    public void SetCheckpoint(int checkpointID)
    {
        SetCheckpoint(checkpointID, false);
    }

    /// <summary>
    /// sets a checkpoint. set forceLower to true to force it to set a lower checkpoint than the current.
    /// </summary>
    public void SetCheckpoint(int checkpointID, bool forceLower)
    {
        if (checkpointID > currCheckpoint || forceLower)
        {
            currCheckpoint = checkpointID;
            foreach (ICheckpointSave checkpointSaver in FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointSave>())
            {
                checkpointSaver.SaveCheckpoint(currCheckpoint);
            }
            PlayerData.currCheckpoint = checkpointID;
        }
    }
}
