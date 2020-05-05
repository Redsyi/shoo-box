using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICityStar : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    [Header("Properties")]
    public float moveTime;

    bool setInitialPosition;
    RectTransform rectTransform;
    Vector2 destAnchor;
    bool animationRunning;

    /// <summary>
    /// sets the position of this star to number "position" of "of". takes care of animation when applicable
    /// </summary>
    public void SetPosition(int position, int of)
    {
        if (!setInitialPosition)
        {
            rectTransform = transform as RectTransform;
            setInitialPosition = true;
            Vector2 anchorPos = new Vector2((position + 1f) * 1f / (of + 1), rectTransform.anchorMax.y);
            rectTransform.anchorMax = anchorPos;
            rectTransform.anchorMin = anchorPos;
            rectTransform.anchoredPosition = Vector2.zero;
        } else
        {
            animator.SetTrigger("Shine");
            destAnchor = new Vector2((position + 1f) * 1f / (of + 1), rectTransform.anchorMax.y);
            if (!animationRunning)
                StartCoroutine(AnimateToNewPosition());
        }
    }

    /// <summary>
    /// animates a star to it's new position
    /// </summary>
    IEnumerator AnimateToNewPosition()
    {
        animationRunning = true;
        float progress = 0;
        float timePassed = 0;
        Vector2 oldAnchor = rectTransform.anchorMin;
        
        while (progress < 1)
        {
            yield return null;
            timePassed += Time.unscaledDeltaTime;
            progress = timePassed / moveTime;
            Vector2 anchor = Vector2.Lerp(oldAnchor, destAnchor, progress);
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.anchoredPosition = Vector2.zero;
        }
        animationRunning = false;
    }
}
