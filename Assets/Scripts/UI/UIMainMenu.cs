using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIMainMenu : MonoBehaviour
{
    public bool options { get; private set; }
    public GameObject initialButton;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject shoebox;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(initialButton);
        optionsMenu.SetActive(options);
    }
    public void PlayButton()
    {
        LevelBridge.BridgeTo(1, "Prologue");
    }

    public void OptionsButton()
    {
        options = !options;
        optionsMenu.SetActive(options);
        mainMenu.SetActive(!options);
        shoebox.SetActive(!options);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
