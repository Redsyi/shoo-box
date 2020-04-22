using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    }

    private void OnTriggerStay(Collider other)
    {
        if (detectPlayer && !foundPlayer)
        {
            foundPlayer = true;
            invokeOnCompleted.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectPlayer)
        {
            detectPlayer = false;
            invokeOnPlayerLeft.Invoke();
            foreach (TutorialPlayerHideDetector detector in FindObjectsOfType<TutorialPlayerHideDetector>())
            {
                detector.gameObject.SetActive(false);
            }
        }
    }
}
