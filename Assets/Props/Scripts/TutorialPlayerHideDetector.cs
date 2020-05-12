using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// class used in the tutorial to detect the player and tell the tutorial that they have successfully hidden / left the hide zone
/// </summary>
public class TutorialPlayerHideDetector : MonoBehaviour
{
    public static bool detectPlayer;
    private float currPlayerTime;
    public float playerTimeRequired;
    private Collider collider;
    public UnityEvent invokeOnCompleted;
    public UnityEvent invokeOnPlayerLeft;
    bool foundPlayer;
    

    private void Start()
    {
        detectPlayer = false;
        currPlayerTime = playerTimeRequired;
    }

    private void OnTriggerStay(Collider other)
    {

        if (detectPlayer)
        {
            currPlayerTime--;
            foundPlayer = true;
            if (currPlayerTime <= 0)
                invokeOnCompleted.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectPlayer)
        {
            if (currPlayerTime <= 0)
            {
                detectPlayer = false;
                invokeOnPlayerLeft.Invoke();
                foreach (TutorialPlayerHideDetector detector in FindObjectsOfType<TutorialPlayerHideDetector>())
                {
                    detector.gameObject.SetActive(false);
                }
            }
            else
                currPlayerTime = playerTimeRequired;
        }
    }
}
