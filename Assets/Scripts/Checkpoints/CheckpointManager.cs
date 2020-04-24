﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CheckpointManager : MonoBehaviour
{
    private static int currLevel = -1;
    public static int currCheckpoint = 0;
    public static Color[] checkpointColors = { Color.blue, Color.red, Color.green, Color.cyan };

    void Start()
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "LevelBridge" && scene.buildIndex != currLevel)
            {
                currLevel = scene.buildIndex;
                currCheckpoint = 0;
                break;
            }
        }
        ReloadCheckpointItems();
    }

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

    public void SetCheckpoint(int checkpointID, bool forceLower)
    {
        if (checkpointID > currCheckpoint || forceLower)
        {
            currCheckpoint = checkpointID;
            foreach (ICheckpointSave checkpointSaver in FindObjectsOfType<MonoBehaviour>().OfType<ICheckpointSave>())
            {
                checkpointSaver.SaveCheckpoint(currCheckpoint);
            }
        }
    }
}
