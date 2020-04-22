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
    public GameObject optionsInitial;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(initialButton);
        optionsMenu.SetActive(options);
    }
    public void PlayButton()
    {
        LevelBridge.BridgeTo("IntroCutscene", "Our story begins here...");
    }

    public void OptionsButton()
    {
        options = !options;
        //optionsMenu.GetComponent<CanvasGroup>().interactable = options;
        optionsMenu.SetActive(options);
        //mainMenu.GetComponent<CanvasGroup>().interactable = !options;
        mainMenu.SetActive(!options);
        shoebox.SetActive(!options);
        if (options)
        {
            //mainMenu.GetComponent<CanvasGroup>().alpha = 0f;
            EventSystem.current.SetSelectedGameObject(optionsInitial);
        } else
        {
            EventSystem.current.SetSelectedGameObject(initialButton);
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
