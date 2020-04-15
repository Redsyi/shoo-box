using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTransformSaver : MonoBehaviour, ICheckpointItem, ICheckpointSave
{
    Vector3 checkpointPosition;
    Quaternion checkpointRotation;
    static Dictionary<string, Vector3> positions;
    static Dictionary<string, Quaternion> rotations;
    string serializedID;

    private void Awake()
    {
        if (positions == null)
            positions = new Dictionary<string, Vector3>();
        if (rotations == null)
            rotations = new Dictionary<string, Quaternion>();
        serializedID = Serialize();
    }

    public void LoadCheckpoint(int checkpointID)
    {
        if (positions.ContainsKey(serializedID))
        {
            transform.position = positions[serializedID];
            positions.Remove(serializedID);
        }
        if (rotations.ContainsKey(serializedID))
        {
            transform.rotation = rotations[serializedID];
            rotations.Remove(serializedID);
        }
    }

    public void SaveCheckpoint(int checkpointID)
    {
        positions[serializedID] = transform.position;
        rotations[serializedID] = transform.rotation;
    }

    public static void ResetAll()
    {
        positions = new Dictionary<string, Vector3>();
        rotations = new Dictionary<string, Quaternion>();
    }

    string Serialize()
    {
        return $"{gameObject.name}{Mathf.FloorToInt(transform.position.x)}{Mathf.FloorToInt(transform.position.y)}{Mathf.FloorToInt(transform.position.z)}";
    }
}
