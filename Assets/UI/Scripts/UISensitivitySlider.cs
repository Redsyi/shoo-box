using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISensitivitySlider : MonoBehaviour
{
    Slider slider;

    void CheckSlider()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    public void LoadSensitivity()
    {
        CheckSlider();

        slider.value = slider.maxValue * PlayerData.normalizedRotateSensitivity;
    }

    public void SaveSensitivity()
    {
        PlayerData.SaveControlSettings();
    }

    public void OnSliderChanged()
    {
        CheckSlider();
        PlayerData.normalizedRotateSensitivity = slider.value / slider.maxValue;
    }
}
