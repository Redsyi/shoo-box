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
    public GameObject backButton;
    public Event pauseSound;
    public Event unpauseSound;
    public Event onPressed;
    public Event onHovered;
    public CanvasGroup menu;
    public CanvasGroup controls;

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    
    public void TogglePause()
    {
        paused = !paused;

        if (paused)
            pauseSound.Post(gameObject);
        else
            unpauseSound.Post(gameObject);

        gameObject.SetActive(paused);

        if (paused)
            EventSystem.current.SetSelectedGameObject(defaultSelected);

        
        Time.timeScale = (paused ? 0 : 1);
        (paused ? pauseSound : unpauseSound)?.Post(gameObject);

    }
    

    public void OnRetryPressed()
    {
        onHovered.Post(gameObject);
        Time.timeScale = 1;
        LevelBridge.Reload("Gave up so easily?");
        onPressed.Post(gameObject);
    }

    public void OnExitPressed()
    {
        onHovered.Post(gameObject);
        Time.timeScale = 1;
        LevelBridge.BridgeTo(0, "See ya later!");
        onPressed.Post(gameObject);
    }

    public void OnControlsPressed()
    {
        /*displayControls = !displayControls;
         menu.alpha = (displayControls ? 0 : 1);
         menu.interactable = displayControls;
         controls.alpha = (displayControls ? 1 : 0);
         controls.interactable = displayControls;*/
        EventSystem.current.SetSelectedGameObject(backButton);
        menu.alpha = 0f;
        menu.interactable = false;
        controls.alpha = 1f;
        controls.interactable = true;

    }

    public void OnBackPressed()
    {
        /*displayControls = !displayControls;
        menu.alpha = (displayControls ? 0 : 1);
        menu.interactable = displayControls;
        controls.alpha = (displayControls ? 1 : 0);*/
        EventSystem.current.SetSelectedGameObject(defaultSelected);
        menu.alpha = 1f;
        menu.interactable = true;
        controls.alpha = 0f;
        controls.interactable = false;
    }
}
