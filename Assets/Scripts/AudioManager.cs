using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static NoiseIndicator noiseIndicatorPrefab;
    public NoiseIndicator initial;

    // Start is called before the first frame update
    void Start()
    {
        noiseIndicatorPrefab = initial;
    }

    public static void MakeNoise(Vector3 pos, float radius, AudioClip clip, float volume)
    {
        NoiseIndicator noise = Instantiate(noiseIndicatorPrefab, pos, Quaternion.identity);
        noise.MakeNoise(radius, clip, volume);
    }

}
