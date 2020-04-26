using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devmode : MonoBehaviour
{
    static int timesLeft = 10;
    public static bool active;
    public AK.Wwise.Event activeSound;

    private void Start()
    {
        timesLeft = 10;
    }

    public void OnDevmode()
    {
        print(timesLeft);
        if (!active)
        {
            timesLeft--;
            if (timesLeft == 0)
            {
                active = true;
                activeSound.Post(gameObject);
                AkSoundEngine.SetRTPCValue("MusicVolume", 0);
            }
        }
    }
}
