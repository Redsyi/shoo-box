using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreditsHandler : MonoBehaviour
{
    [SerializeField] Animator fromLeft;
    [SerializeField] Animator fromRight;
    [SerializeField] float timeCreditsShown;
    [SerializeField] float disanceBetweenCredits;
    [SerializeField] GameObject roleObject;
    [SerializeField] string role;
    [SerializeField] List<string> names;
    private bool isPopulatorDone = false;
    /*

        Script Goals: alternate between showing credits on left and right
                      side of the screen
        Currently: Right spawns before left for some reason and then after
                   left plays it unhooks from the fromLeft field in the inspector

                   Probably bc I'm calling this script, and thus coroutines,
                   multiple times, and I need to run in through a list instead?
                   Apologies for the mess, I wrote most of this while half asleep.

    */
    // Start is called before the first frame update
    void Start()
    {
        NamesPopulator(names);
        RoleTyper();
        StartCoroutine(CreditsDistanceWaiter());
    }

    private void Update()
    {
        //CheckAnimation(fromLeft, fromRight);
    }

    //Check distance except this is measured in seconds so not really
    //Run check animation function afterward to determine next thing.
    IEnumerator CreditsDistanceWaiter()
    {
        yield return new WaitForSecondsRealtime(disanceBetweenCredits);
        CheckAnimation(fromLeft, fromRight);
    }

    IEnumerator CreditsTimerLeft()
    {
        yield return new WaitForSeconds(timeCreditsShown);
        fromLeft.SetTrigger("animateOut");
    }

    IEnumerator CreditsTimerRight()
    {
        yield return new WaitForSeconds(timeCreditsShown);
        fromRight.SetTrigger("animateOut");
    }

    public void CheckAnimation(Animator left, Animator right)
    {
        if (left != null && right == null)
        {
            fromLeft.SetTrigger("animateIn");
            StartCoroutine(CreditsTimerLeft());
        }

        if (right != null && left == null)
        {
            fromRight.SetTrigger("animateIn");
            StartCoroutine(CreditsTimerRight());
        }

    }

    public void RoleTyper()
    {
        roleObject.GetComponent<TMPro.TextMeshProUGUI>().text = role;
    }

    public void NamesPopulator(List<string> oneName)
    {
        if (oneName == null)
        {
            oneName = new List<string>();
            Debug.Log("List created.");

            foreach (string aName in oneName)
            {
                oneName.Add(aName);
                Debug.Log("adding: " + aName);
            }
        }

        isPopulatorDone = true;
        if (isPopulatorDone)
        {
            NamesTyper();
        }

    }

    public void NamesTyper()
    {
        var currentText = GameObject.Find("Names");
        currentText.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        for (int i = 0; i < names.Count; i++)
        {
            currentText.GetComponent<TMPro.TextMeshProUGUI>().text += names[i] + "\n";
        }
    }
}
