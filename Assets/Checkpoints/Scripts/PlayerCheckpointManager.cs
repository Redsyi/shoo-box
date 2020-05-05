using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that sets certain player properties on a checkpoint load.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerCheckpointManager : MonoBehaviour, ICheckpointItem
{
    public PlayerCheckpoint[] checkpoints;

    public void LoadCheckpoint(int checkpointID)
    {
        if (checkpointID >= 0 && checkpointID < checkpoints.Length)
        {
            LoadCheckpointSafe(checkpointID);
        }
        else if (checkpointID >= checkpoints.Length && checkpoints.Length != 0)
        {
            LoadCheckpointSafe(checkpoints.Length - 1);
        }
    }

    private void LoadCheckpointSafe(int checkpointID)
    {
        Player player = GetComponent<Player>();
        player.wigglesRequired = checkpoints[checkpointID].wigglesRequired;
        player.lockChangeForm = checkpoints[checkpointID].lockChangeForm;
        player.lockShoeSight = checkpoints[checkpointID].lockShoeSight;
        if (checkpoints[checkpointID].setLegForm && checkpoints[checkpointID].legForm)
        {
            player.OnChangeForm();
        }
        player.startingShoes = checkpoints[checkpointID].startingShoes;
        player.EquipStartingShoes();
        if (checkpoints[checkpointID].resetCamera)
        {
            //need a one frame delay on this because some cutscenes can still interfere
            StartCoroutine(ResetCameraDelayed());
        }
    }

    IEnumerator ResetCameraDelayed()
    {
        yield return null;
        CameraScript.current.camera.orthographicSize = (Player.current.legForm ? CameraScript.current.farZoomLevel : CameraScript.current.closeZoomLevel);
    }
}

[System.Serializable]
public class PlayerCheckpoint {
    public int wigglesRequired;
    public bool lockChangeForm;
    public bool lockShoeSight;
    public bool setLegForm;
    public bool legForm;
    public ShoeType[] startingShoes;
    public bool resetCamera = true;
}