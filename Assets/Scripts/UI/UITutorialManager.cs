using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialManager : MonoBehaviour
{

    private bool movement;
    private bool camera;
    private bool legform;
    private bool interact;
    private bool use;
    private Animator currAnimator;
    private ShoePickup boots;
    
    public Animator movementAni;
    public Animator cameraAni;
    public Animator legformAni;
    public Animator interactAni;
    public Animator useAni;
    public bool controller;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShowMovement", 2);
        boots = FindObjectOfType<ShoePickup>();
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

    public void ShowLegform()
    {
        if (camera && !legform)
        {
            cameraAni.gameObject.SetActive(false);
            legform = true;
            legformAni.gameObject.SetActive(true);
            currAnimator = legformAni;
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
        }
    }
}
