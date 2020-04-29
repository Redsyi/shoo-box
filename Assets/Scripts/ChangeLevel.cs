using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public string scene;
    public bool canChangeLevels = true;
    public bool lockChange;
    public float delay;
    public string message;

    public void OnTriggerEnter(Collider other)
    {
        if (canChangeLevels && other.gameObject.CompareTag("Player"))
        {
            canChangeLevels = false;
            if (delay == 0f)
                DoChangeLevel();
            else
                Invoke("DoChangeLevel", delay);
        }
    }

    void DoChangeLevel()
    {
        LevelBridge.BridgeTo(scene, message);
    }
}
