using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIPauseMenu : MonoBehaviour
{
    public bool paused { get; private set; }
    public static UIPauseMenu instance;
    public AudioClip pauseSound;
    public AudioClip unpauseSound;
    public AudioClip confirmSound;
    public GameObject defaultSelected;

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        paused = !paused;

        if(paused)
            AudioManager.instance.Pause();
        else
            AudioManager.instance.UnPause();

        gameObject.SetActive(paused);

        if (paused)
            EventSystem.current.SetSelectedGameObject(defaultSelected);

        Time.timeScale = (paused ? 0 : 1);
        AudioManager.MakeNoise(Vector3.zero, 0, (paused ? pauseSound:unpauseSound), 0.65f);

    }

    public void OnRetryPressed()
    { 
        Time.timeScale = 1;
        AudioManager.MakeNoise(Vector3.zero, 0, confirmSound, 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void OnExitPressed()
    { 
        Time.timeScale = 1;
        AudioManager.MakeNoise(Vector3.zero, 0, confirmSound, 1);
        SceneManager.LoadScene(0);
    }
}
