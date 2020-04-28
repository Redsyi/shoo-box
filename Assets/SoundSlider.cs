using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    public SoundSettings volumeType;
    Slider slider;

    public void OnValueChanged(float volume)
    {
        AkSoundEngine.SetRTPCValue(PlayerData.GetVolumeRTPCName(volumeType), volume * 100 / slider.maxValue);
    }

    public void LoadFromPlayerPrefs()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        slider.value = PlayerData.GetVolume(volumeType) * slider.maxValue;
    }

    public void SaveToPlayerPrefs()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
        PlayerData.SetVolume(volumeType, slider.value / slider.maxValue);
    }
}
