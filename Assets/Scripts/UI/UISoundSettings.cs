using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundSettings : MonoBehaviour
{
    SoundSlider[] sliders;

    public void LoadSliderValues()
    {
        if (sliders == null)
            sliders = GetComponentsInChildren<SoundSlider>();
        foreach (SoundSlider slider in sliders)
        {
            slider.LoadFromPlayerPrefs();
        }
    }
    
    public void SaveSliderValues()
    {
        if (sliders == null)
            sliders = GetComponentsInChildren<SoundSlider>();
        foreach (SoundSlider slider in sliders)
        {
            slider.SaveToPlayerPrefs();
        }
    }
}
