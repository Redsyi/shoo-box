using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CheckpointManager : MonoBehaviour
{
    private static int currLevel = -1;
    private static int currCheckpoint = 0;
    public static Color[] checkpointColors = { Color.blue, Color.red, Color.green, Color.cyan };

    void Start()
    {
        int level = SceneManager.GetActiveScene().buildIndex;
        if (level != currLevel)
        {
            currLevel = level;
            currCheckpoint = 0;
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
        if (checkpointID > currCheckpoint)
            currCheckpoint = checkpointID;
    }
}
