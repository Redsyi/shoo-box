using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveTracker : MonoBehaviour
{
    public string[] objectives;
    private int currObjective;
    private int currAnimatedObjective;
    public static ObjectiveTracker instance;
    private bool animating;
    public Animator animator;
    public TextMesh text;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (!animating && currAnimatedObjective < currObjective)
        {
            ++currAnimatedObjective;
            animating = true;
        }
    }

    IEnumerator AnimateObjective()
    {
        animating = true;
        animator.SetTrigger("Complete");
        yield return null;
    }

    public void CompleteObjective(int objectiveNum)
    {
        if (objectiveNum > currObjective)
            currObjective = objectiveNum;
    }
}
