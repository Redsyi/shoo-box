using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : MonoBehaviour
{
    public bool paused { get; private set; }
    public static UIPauseMenu instance;
    public AudioClip pauseSound;

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        paused = !paused;
        gameObject.SetActive(paused);
        Time.timeScale = (paused ? 0 : 1);
        AudioManager.MakeNoise(Vector3.zero, 0, pauseSound, 0.65f);
    }

    public void OnRetryPressed()
    {
        Debug.Log("Retry");
    }

    public void OnExitPressed()
    {
        Debug.Log("Exit");
    }
}
