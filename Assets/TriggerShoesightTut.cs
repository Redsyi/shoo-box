using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerShoesightTut : MonoBehaviour
{
    public UITutorialManager tutorial;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorial.DoShoesightColorTutorial();
            Destroy(gameObject);
        }
    }
}
