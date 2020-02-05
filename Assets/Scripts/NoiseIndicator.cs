using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseIndicator : MonoBehaviour
{
    public AudioSource source;

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
            foreach (Maid AI in FindObjectsOfType<Maid>())
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

    private void Sudoku()
    {
        Destroy(gameObject);
    }
}
