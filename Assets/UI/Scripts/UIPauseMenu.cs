﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Event = AK.Wwise.Event;
using UnityEngine.UI;

/// <summary>
/// class to manage the pause menu, mainly a wrapper for a UIPaneManager
/// </summary>
public class UIPauseMenu : MonoBehaviour
{
    public bool paused { get; private set; }
    public static UIPauseMenu instance;
    public Event pauseSound;
    public Event unpauseSound;
    public UIPaneManager panes;
    
    public GameObject toggleController;
    public GameObject toggleKeyboard;

    public Image backgroundPane;
    public Color paneColor;
    public Color paneHiddenColor;

    private string toggleText;
    public bool conToggled { get; private set; }

    private void Start()
    {
        instance = this;
        conToggled = false;
    }

    /// <summary>
    /// toggles the game's paused state
    /// </summary>
    public void TogglePause()
    {
        paused = !paused;

        if (paused)
            panes.Appear();
        else
            panes.Disappear();

        Time.timeScale = (paused ? 0 : 1);
        (paused ? pauseSound : unpauseSound)?.Post(gameObject);

        backgroundPane.color = (paused ? paneColor : paneHiddenColor);

        if (paused)
            Player.current.inputSystem.SwitchCurrentActionMap("UI");
        else
            Player.current.inputSystem.SwitchCurrentActionMap("Player");
    }

    public void NextTab()
    {
        panes.NextPane();
    }

    public void PrevTab()
    {
        panes.PreviousPane();
    }


    public void OnRetryPressed()
    {
        Time.timeScale = 1;
        LevelBridge.Reload("Gave up so easily?");
    }

    public void OnExitPressed()
    {
        Time.timeScale = 1;
        LevelBridge.BridgeTo("MainMenu", "See ya later!");
    }

    public void ControlsPaneLoaded()
    {
        bool usingGamepad = Controls.usingController;
        toggleController.SetActive(usingGamepad);
        toggleKeyboard.SetActive(!usingGamepad);
    }
}