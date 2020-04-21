using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    [SerializeField] string rtpcName;

    public void OnValueChanged(float volume)
    {
        AkSoundEngine.SetRTPCValue(rtpcName, volume);
    }
}
