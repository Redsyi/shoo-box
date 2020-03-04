using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static NoiseIndicator noiseIndicatorPrefab;
    public NoiseIndicator initial;
    public AudioSource gameMusic;
    public AudioSource menuMusic;
    public static AudioManager instance;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        noiseIndicatorPrefab = initial;
    }

    /// <summary>
    /// Make a noise in the world
    /// </summary>
    /// <param name="pos">World position of the noise</param>
    /// <param name="radius">How far away AI will notice the noise, 0 for "silent"</param>
    /// <param name="clip">AudioClip to play, null for nothing</param>
    /// <param name="volume">Volume (range 0..1) at which to play the AudioClip</param>
    public static void MakeNoise(Vector3 pos, float radius, AudioClip clip, float volume)
    {
        NoiseIndicator noise = Instantiate(noiseIndicatorPrefab, pos, Quaternion.identity);
        noise.MakeNoise(radius, clip, volume);
    }

    public void Pause()
    {
        if(gameMusic)
            gameMusic.volume = 0;
        if(menuMusic)
            menuMusic.volume = 1;
    }

    public void UnPause()
    {
        if(gameMusic)
            gameMusic.volume = 1;
        if(menuMusic)
            menuMusic.volume = 0;
    }


}
