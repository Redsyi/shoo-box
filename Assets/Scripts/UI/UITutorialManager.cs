﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialManager : MonoBehaviour
{
    public static UITutorialManager instance;
    private bool movement;
    private bool camera;
    private bool legform;
    private bool interact;
    private bool use;
    private Animator currAnimator;
    private ShoePickup boots;
    private Player player;
    
    public Animator movementAni;
    public Animator cameraAni;
    public Animator legformAni;
    public Animator interactAni;
    public Animator useAni;
    [HideInInspector]
    public bool controller;
    private static bool finishedTutorial;

    public DummyFixable toiletDummyFixable;
    public DummyFixable showerDummyFixable;
    public StealFocusWhenSeen toiletFocusStealer;
    public StealFocusWhenSeen initialFocusStealer;
    public GameObject hidePointers;
    public UIPopup hideTutorialPopup;
    public StealFocusWhenSeen maidFocusStealer;
    public UIPopup changeFormPopup;
    public Animator objectiveTrackerAnimator;
    public UIPopup kickPopup;

    public AIAgent dad;
    public GameObject dadHallwayInvestigatePoint;
    public AIPatrolPoint[] dadRoomInvestigatePoints;
    private bool dadDoingRoomCheck;

    public AK.Wwise.Event onUIPopUp;
    public AK.Wwise.Event onSpaceBar;

    void Start()
    {
        instance = this;
        if (!finishedTutorial)
        {
            Invoke("ShowMovement", 2);
        }
        boots = FindObjectOfType<ShoePickup>();
        StartCoroutine(ActivateShower());
        player = Player.current;
    }

    void Update()
    {
        if (currAnimator)
        {
            currAnimator.SetBool("Controller", controller);
        }

        if (interactAni && boots)
        {
            interactAni.transform.position = CameraScript.current.camera.WorldToScreenPoint(boots.transform.position) + Vector3.down * 150;
        }
    }

    public void CancelTutorial()
    {
        CancelInvoke("ShowMovement");
    }

    void ShowMovement()
    {

        if(!movement)
        {
            movement = true;
            movementAni.gameObject.SetActive(true);
            currAnimator = movementAni;
        }
    }

    public void ShowCamera()
    {
        if (movement && !camera)
        {
            movementAni.gameObject.SetActive(false);
            camera = true;
            cameraAni.gameObject.SetActive(true);
            currAnimator = cameraAni;
        }
    }

    public void FinishCamera()
    {
        if (camera)
        {
            cameraAni.gameObject.SetActive(false);
        }
    }

    public void ShowLegform()
    {
        if (camera && !legform)
        {
            legform = true;
            legformAni.gameObject.SetActive(true);
            currAnimator = legformAni;
        }
    }

    public void DidLegForm()
    {
        if (legform)
        {
            legformAni.gameObject.SetActive(false);
            PlayerStoodUp();
        }
    }

    public void ShowInteract()
    {
        if (legform && !interact)
        {
            legformAni.gameObject.SetActive(false);
            interact = true;
            interactAni.gameObject.SetActive(true);
            currAnimator = interactAni;
        }
    }

    public void ShowUse()
    {
        if (interact && !use)
        {
            interactAni.gameObject.SetActive(false);
            use = true;
            useAni.gameObject.SetActive(true);
            currAnimator = useAni;
        }
    }

    public void FinishUse()
    {
        if (use)
        {
            useAni.gameObject.SetActive(false);
            currAnimator = null;
            finishedTutorial = true;
        }
    }

    public void SwitchToShower()
    {
        StartCoroutine(ShowerSwitch());
    }

    public void ShowHideTutorial()
    {
        hideTutorialPopup.Activate();
        onUIPopUp.Post(gameObject);
        hidePointers.SetActive(true);
        TutorialPlayerHideDetector.detectPlayer = true;
        //onSpaceBar.Post(gameObject);
    }

    public void PlayerHid()
    {
        if (showerDummyFixable)
        {
            hidePointers.SetActive(false);
            Destroy(showerDummyFixable.gameObject);
            maidFocusStealer.Trigger();
        }
    }

    public void MaidLeft()
    {
        changeFormPopup.Activate();
        onUIPopUp.Post(gameObject);
        player.lockChangeForm = false;
    }

    public void PlayerStoodUp()
    {
        StartCoroutine(ShowObjectiveList());
    }

    public void DoKickTutorial()
    {
        kickPopup.Activate();
        onUIPopUp.Post(gameObject);
    }

    public void TeachShoeSight()
    {
        StartCoroutine(ShowShoeSight());
    }

    IEnumerator ShowShoeSight()
    {
        yield return new WaitForSeconds(2f);
        player.lockShoeSight = false;
        UIShoeSightReminder.instance.Trigger();
    }

    IEnumerator ShowObjectiveList()
    {
        yield return new WaitForSeconds(1);
        objectiveTrackerAnimator.SetTrigger("Activate");
    }

    IEnumerator ShowerSwitch()
    {
        yield return new WaitForSeconds(1.6f);
        Destroy(toiletDummyFixable.gameObject);
    }

    IEnumerator ActivateShower()
    {
        yield return null;
        yield return null;
        showerDummyFixable.broken = true;
    }

    public void DadInvestigateHallway()
    {
        StartCoroutine(DoDadInvestigateHallway());
    }

    IEnumerator DoDadInvestigateHallway()
    {
        ScriptedSequence dadSequence = dad.GetComponent<ScriptedSequence>();
        dadSequence.Interrupt();
        dad.Investigate(dadHallwayInvestigatePoint);
        yield return new WaitForSeconds(6);
        dadSequence.Trigger();
        yield return new WaitForSeconds(4);
        dadSequence.Interrupt();
    }

    public void DadRoomCheck()
    {
        if (!dadDoingRoomCheck)
            StartCoroutine(DoDadRoomCheck());
    }

    IEnumerator DoDadRoomCheck()
    {
        dadDoingRoomCheck = true;
        foreach (AIPatrolPoint point in dadRoomInvestigatePoints)
        {
            while (dad.currState.state == AIState.INVESTIGATE || dad.currState.state == AIState.INVESTIGATING || dad.currState.state == AIState.INTERACT)
                yield return null;
            dad.Investigate(point.gameObject, investigateTime: 2);
        }
        dadDoingRoomCheck = false;
    }
}
