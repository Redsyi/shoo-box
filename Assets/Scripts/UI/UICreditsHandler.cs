using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreditsHandler : MonoBehaviour
{
    public Animator fromLeft;
    public Animator fromRight;
    public float timeCreditsShown;
    public float distanceBetweenCredits;
    public GameObject leftRoleObject;
    public GameObject rightRoleObject;
    public GameObject leftName;
    public GameObject rightName;
    public string[] role; // List of all the roles. Role index matches up with name index
    public string[] names; // List of all the names 
    private int index;
    private float distanceTimer; // Timer for distance between credits
    private float creditTimer; // Timer for duration credits shown
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

        //distanceTimer = distanceBetweenCredits;
        // creditTimer = timeCreditsShown;
        print("Starting.");
        print("Distance between credits:" + distanceBetweenCredits);
        print("Time credits shown: " + timeCreditsShown);
        if (names.Length != role.Length)
            Debug.LogError("names and role length do not match.");
        else
        {
            SetNext();
            Invoke("CreditsDistanceWaiter", distanceBetweenCredits);
        }
        
        //NamesPopulator(names);
        // StartCoroutine(CreditsDistanceWaiter());

    }

    public void DisplayTag() 
    {
        // If the current index is odd, that means
        // we have the left tag set since after setting
        // we increase the index by 1 and vice versa.
        if (index % 2 != 0)
        {
            fromLeft.SetTrigger("animateIn");
            print("Coming in from left.");
            print("Role, Name: " + leftRoleObject.GetComponent<TMPro.TextMeshProUGUI>().text + ", " + leftName.GetComponent<TMPro.TextMeshProUGUI>().text);
            Invoke("CreditsTimerLeft", timeCreditsShown);
        }
        else
        {
            fromRight.SetTrigger("animateIn");
            print("Coming in from right.");
            print("Role, Name: " + rightRoleObject.GetComponent<TMPro.TextMeshProUGUI>().text + ", " + rightName.GetComponent<TMPro.TextMeshProUGUI>().text);
            Invoke("CreditsTimerRight", timeCreditsShown);
        }
    }

    public void SetNext()
    {
        NextName(); // Setup first name to be shown
        NextRole(); // Setup first role to be shown
        index++;
    }

    public void NextName()
    {   
        // Odd indices go to the right and vice versa
        if (index % 2 != 0 && index < role.Length)
            rightName.GetComponent<TMPro.TextMeshProUGUI>().text = names[index];
        else if (index % 2 == 0 && index < role.Length)
            leftName.GetComponent<TMPro.TextMeshProUGUI>().text = names[index];
    }

    public void NextRole()
    {
        // Odd indices go to the right and vice versa
        if (index % 2 != 0 && index < role.Length)
            rightRoleObject.GetComponent<TMPro.TextMeshProUGUI>().text = role[index];
        else if (index % 2 == 0 && index < role.Length)
            leftRoleObject.GetComponent<TMPro.TextMeshProUGUI>().text = role[index];
    }

    //Check distance except this is measured in seconds so not really
    //Run check animation function afterward to determine next thing.
    public void CreditsDistanceWaiter()
    {
        //print("Time before waiting");
        //yield return new WaitForSecondsRealtime(distanceBetweenCredits);
        if (index <= names.Length)
        {
            print("Displaying tag");
            print("Index: " + index);
            DisplayTag();
        }
        else
            Debug.LogError("Index out of bounds");
            
    }

    /*public void CreditsTimer(Animator animator)
    {
        //yield return new WaitForSeconds(timeCreditsShown);
        animator.SetTrigger("animateOut");
        SetNext();
        CreditsDistanceWaiter();

    }*/

    public void CreditsTimerLeft()
    {
        //yield return new WaitForSeconds(timeCreditsShown);
        fromLeft.SetTrigger("animateOut");
        SetNext();
        Invoke("CreditsDistanceWaiter", distanceBetweenCredits);
    }
    public void CreditsTimerRight()
    {
        //yield return new WaitForSeconds(timeCreditsShown);
        fromRight.SetTrigger("animateOut");
        SetNext();
        Invoke("CreditsDistanceWaiter", distanceBetweenCredits);
    }

    /*public void CheckAnimation(Animator left, Animator right)
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

    }*/


    /*  public void NamesPopulator(List<string> oneName)
      {
          print("Populating names");
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
          else
              Debug.LogError("oneName is null");


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
          for (int i = 0; i < names.Length; i++)
          {
              currentText.GetComponent<TMPro.TextMeshProUGUI>().text += names[i] + "\n";
          }
      }*/
}
