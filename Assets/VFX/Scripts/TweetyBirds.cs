using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages the effect for the "tweety birds" around an npc's head when they're stunned
/// </summary>
public class TweetyBirds : MonoBehaviour
{
    public float rotationSpeed;
    public float maxDeviation;
    public float deviationSpeed;
    public Transform[] birds;

    bool on;
    
    void Update()
    {
        if (on)
        {
            //essentially, each "bird" is on an "invisible boom stick", which is angled with a sine wave and rotated around the center
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + rotationSpeed * Time.deltaTime);

            for (int i = 0; i < birds.Length; ++i)
            {
                float sineOffset = (i * 2 * Mathf.PI);
                float angleOffset = Mathf.Sin((Time.time * deviationSpeed) + sineOffset) * maxDeviation;
                birds[i].localEulerAngles = new Vector3(angleOffset, birds[i].localEulerAngles.y);
            }
        }
    }

    public void Disable()
    {
        on = false;
        foreach (Transform bird in birds)
        {
            bird.gameObject.SetActive(false);
        }
    }

    public void Enable()
    {
        on = true;
        foreach (Transform bird in birds)
        {
            bird.gameObject.SetActive(true);
        }
    }
}
