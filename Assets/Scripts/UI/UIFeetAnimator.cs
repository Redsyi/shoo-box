using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFeetAnimator : MonoBehaviour
{
    private class FootAnimation
    {
        public float offset;
        public Image image;
        public float rateMultiplier;
    }
    private List<FootAnimation> feet;
    public float speed;
    
    void Start()
    {
        feet = new List<FootAnimation>();
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img)
            {
                FootAnimation foot = new FootAnimation();
                foot.image = img;
                foot.offset = Random.Range(0, 2 * Mathf.PI);
                foot.rateMultiplier = Random.Range(0.8f, 1.2f);
                feet.Add(foot);
            }
        }
    }
    
    void Update()
    {
        foreach (FootAnimation foot in feet)
        {
            float adjustedTime = foot.offset + (Time.realtimeSinceStartup * speed * foot.rateMultiplier);
            float alpha = (Mathf.Sin(adjustedTime) + 1) / 2;
            foot.image.color = new Color(0, 0, 0, alpha);
        }
    }
}