using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//deprecated
public class ControlsMenu : MonoBehaviour
{

    bool isGamepad;
    bool oldIsGamepad;
    [SerializeField] RectTransform keyboardControls;
    [SerializeField] RectTransform gamepadControls;

    // Start is called before the first frame update
    void Start()
    {
       isGamepad = CheckGamepad();
       oldIsGamepad = isGamepad;
       UpdateGamePad();
    }

    // Update is called once per frame
    void Update()
    {
        oldIsGamepad = isGamepad;
        isGamepad = CheckGamepad();
        if (isGamepad != oldIsGamepad)
        {
            UpdateGamePad();
        }
    }

    private void UpdateGamePad()
    {
        if (isGamepad)
        {
            keyboardControls.gameObject.SetActive(false);
            gamepadControls.gameObject.SetActive(true);
        }
        else
        {
            keyboardControls.gameObject.SetActive(true);
            gamepadControls.gameObject.SetActive(false);
        }
    }

    private bool CheckGamepad()
    {
        string[] temp = Input.GetJoystickNames();

        //Check whether array contains anything
        if (temp.Length > 0)
        {
            //Iterate over every element
            for (int i = 0; i < temp.Length; ++i)
            {
                //Check if the string is empty or not
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    //Not empty, controller temp[i] is connected
                    return true;
                }
            }
        }
        return false;
    }
}
