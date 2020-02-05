using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseIndicator : MonoBehaviour
{
    public AudioSource source;
    public SpriteRenderer waveRenderer;

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

    private void Sudoku()
    {
        Destroy(gameObject);
    }
}
