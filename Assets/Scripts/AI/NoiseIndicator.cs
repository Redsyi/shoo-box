using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseIndicator : MonoBehaviour
{
    public AudioSource source;
    public SpriteRenderer waveRenderer;

    private void Start()
    {
        if (source == null)
            Debug.LogWarning("Noise Indicator has null AudioSource!");
        if (waveRenderer == null)
            Debug.LogWarning("Noise Indicator has null SpriteRenderer!");
    }

    /// <summary>
    /// Tell this indicator to perform its noise
    /// </summary>
    /// <param name="radius">distance at which AI can hear this noise</param>
    /// <param name="clip">audioclip to player, null for nothing</param>
    /// <param name="volume">volume to play clip at (range 0..1)</param>
    public void MakeNoise(float radius, AudioClip clip, float volume)
    {
        if (clip)
        {
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }

        if (radius > 0)
        {
            StartCoroutine(AnimateCircle(radius));
            foreach (AIAgent AI in FindObjectsOfType<AIAgent>())
            {
                Vector3 distance = AI.transform.position - transform.position;
                if (distance.sqrMagnitude <= radius * radius)
                {
                    AI.Investigate(gameObject);
                }
            }
        }
        Invoke("Sudoku", 5);
    }

    //animates the sound wave
    private IEnumerator AnimateCircle(float radius)
    {
        float size = 0;
        while(size < radius)
        {
            waveRenderer.transform.localScale = new Vector3(size/10f, size/10f, 1);
            size += Time.deltaTime * radius * 2;
            yield return null;
        }
        waveRenderer.enabled = false;
    }

    //destroys gameobject to avoid clutter
    private void Sudoku()
    {
        Destroy(gameObject);
    }
}
