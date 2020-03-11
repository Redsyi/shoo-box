using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIMainMenu : MonoBehaviour
{
    public GameObject initialButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(initialButton);
    }
    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }
}
