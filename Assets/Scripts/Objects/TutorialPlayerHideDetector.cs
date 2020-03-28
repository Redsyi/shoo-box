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

    private void Start()
    {
        detectPlayer = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (detectPlayer)
        {
            currPlayerTime += Time.deltaTime;
            if (currPlayerTime >= playerTimeRequired)
            {
                invokeOnCompleted.Invoke();
                detectPlayer = false;
                foreach (TutorialPlayerHideDetector detector in FindObjectsOfType<TutorialPlayerHideDetector>())
                {
                    detector.gameObject.SetActive(false);
                }
            }
        } else
        {
            currPlayerTime = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currPlayerTime = 0f;
    }
}
