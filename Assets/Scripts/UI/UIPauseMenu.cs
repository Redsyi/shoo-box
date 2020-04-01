using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Event = AK.Wwise.Event;

public class UIPauseMenu : MonoBehaviour
{
    public bool paused { get; private set; }
    public static UIPauseMenu instance;
    public GameObject defaultSelected;
    public Event onPause;
    public Event pauseSound;
    public Event unpauseSound;

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        paused = !paused;

        if (paused)
            //AudioManager.instance.Pause();
            onPause.Post(gameObject);
        else
            //AudioManager.instance.UnPause()；

            gameObject.SetActive(paused);
        if(paused)
            AudioManager.instance.Pause();
        else
            AudioManager.instance.UnPause();

        gameObject.SetActive(paused);

        if (paused)
            EventSystem.current.SetSelectedGameObject(defaultSelected);

        Time.timeScale = (paused ? 0 : 1);
        (paused ? pauseSound : unpauseSound)?.Post(gameObject);

    }

    public void OnRetryPressed()
    { 
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void OnExitPressed()
    { 
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
