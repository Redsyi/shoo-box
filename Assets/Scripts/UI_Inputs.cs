using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Inputs : MonoBehaviour
{
    public bool isPaused;
    public GameObject pauseMenu;
    private GameObject player;
    public GameObject shoeTagActive;
    public GameObject shoeTagInactive;
    private bool bootsOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //test tag inputs (spaghetti code)
        if (Input.GetKeyDown(KeyCode.O))
        {
            //test switch tags
            StartCoroutine(TagSwitcher());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //test cant switch tags
            shoeTagActive.GetComponent<Animator>().SetTrigger("CantSwitch");
            if (bootsOn)
            {
                shoeTagInactive.GetComponent<Animator>().SetTrigger("CantSwitch");
            }
        }
    }

    public void WearShoes()
    {
        StartCoroutine(TagSwitcher());
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
        Time.timeScale = (isPaused ? 0 : 1);
        
    }

    private IEnumerator TagSwitcher()
    {
        shoeTagActive.GetComponent<Animator>().SetTrigger("Switch");
        yield return new WaitForSeconds(0.3f);
        //set active tag inactive
        shoeTagActive.SetActive(false);
        //set inactive tag active
        shoeTagInactive.SetActive(true);
        bootsOn = true;

        //switch active and inactive tags
        //OR just give them their actual names (combatBoots, noShoes, etc)

    }
}
