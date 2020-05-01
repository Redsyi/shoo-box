using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that creates a visible noise that is noticed by npcs.
/// used to play a sound as well but that is deprecated as of wwise integration
/// </summary>
public class NoiseIndicator : MonoBehaviour
{
    public SpriteRenderer waveRenderer;

    private void Start()
    {
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

        if (radius > 0)
        {
            StartCoroutine(AnimateCircle(radius));
            foreach (AIAgent AI in FindObjectsOfType<AIAgent>())
            {
                Vector3 distance = AI.transform.position - transform.position;
                if (AI.interests.Length > 0 && !AI.deaf && distance.sqrMagnitude <= radius * radius)
                {
                    AI.Investigate(gameObject);
                }
            }
        } else
        {
            waveRenderer.enabled = false;
        }
        Invoke("Sudoku", 25);
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
