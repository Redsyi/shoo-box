using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    private bool faded = false;
    public float duration = .4f;
    
    public void Fade()
    {
        print("Fading objective list in");
        CanvasGroup group = GetComponent<CanvasGroup>();

        if (group)
        {
            print("Found group");
            StartCoroutine(DoFade(group, group.alpha, faded ? 0 : 1));
        }
            
        else
            Debug.LogError("Can't find canvas group");

        faded = !faded; // Toggle fade
    }

    public IEnumerator DoFade(CanvasGroup group, float start, float end)
    {
        print("End: " + end);
        float counter = 0f;

        while(counter < duration)
        {
            counter += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, counter / duration);

            yield return null;
        }
    }
}
