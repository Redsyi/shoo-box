using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITSAAlert : MonoBehaviour
{
    public GameObject alertNotification;

    private void Start()
    {
        StartCoroutine(CheckActivation());
    }

    IEnumerator CheckActivation()
    {
        bool somethingAlerting = false;
        while (true)
        {
            somethingAlerting = false;
            foreach (TSAAlert alert in FindObjectsOfType<TSAAlert>())
            {
                if (alert.alertTimeRemaining <= 0f)
                {
                    somethingAlerting = true;
                    break;
                }
            }
            alertNotification.SetActive(somethingAlerting);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
