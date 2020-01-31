using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Inputs : MonoBehaviour
{
    public bool isPaused;
    public GameObject pauseMenu;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPauseMenu(InputValue value)
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        //disable player controls
        if (isPaused)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<Player>().OnMove( );
        }
        
    }
}
