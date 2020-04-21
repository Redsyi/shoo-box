using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] string rtpcName;

    public Slider thisSlider;
    public float masterVolume;
    public float musicVolume;
    public float SFXVolume;


    public void SetSpecificVolume(string whatValue)
    {
        float sliderValue = thisSlider.value;
        
        if (whatValue == "Master")
        {
            masterVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MatserVolume", masterVolume);
        }

        if (whatValue == "Music")
        {
            masterVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MusicVolume", masterVolume);
        }

        if (whatValue == "Master")
        {
            masterVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MatserVolume", masterVolume);
        }

        //AkSoundEngine.SetRTPCValue(rtpcName, volume);
    }
}
