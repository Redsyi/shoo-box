using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour, IKickable, IAIInteractable
{
    private bool on;
    public Color ambientLightOnColor;
    public Color ambientLightOffColor;
    public Transform[] lightsRoots;
    public AIInterest[] interestMask;

    private void Start()
    {
        on = true;
    }
    public void AIFinishInteract()
    {
        if (!on)
        {
            on = true;
            RenderSettings.ambientLight = ambientLightOnColor;
            CameraScript.current.camera.backgroundColor = Color.white;
            foreach (Transform lightsRoot in lightsRoots)
            {
                foreach (Transform lightTransform in lightsRoot)
                {
                    Light light = lightTransform.GetComponent<Light>();
                    if (light)
                    {
                        light.enabled = false;
                    }
                }
            }
            AIAgent.blindAll = false;
            ChangeLevel changeLevel = FindObjectOfType<ChangeLevel>();
            if (changeLevel)
            {
                changeLevel.canChangeLevels = false;
            }
        }
    }

    public void AIInteracting(float interactProgress)
    {

    }

    public float AIInteractTime()
    {
        return 2f;
    }

    public AIInterest[] InterestingToWhatAI()
    {
        return interestMask;
    }

    public bool NeedsInteraction()
    {
        return !on;
    }

    public void OnKick(GameObject kicker)
    {
        if (on)
        {
            on = false;
            RenderSettings.ambientLight = ambientLightOffColor;
            CameraScript.current.camera.backgroundColor = Color.black;
            foreach (Transform lightsRoot in lightsRoots)
            {
                foreach (Transform lightTransform in lightsRoot)
                {
                    Light light = lightTransform.GetComponent<Light>();
                    if (light)
                    {
                        light.enabled = false;
                    }
                }
            }
            AIAgent.blindAll = true;
            ChangeLevel changeLevel = FindObjectOfType<ChangeLevel>();
            if (changeLevel)
            {
                print("enabling changeLevel");
                changeLevel.canChangeLevels = true;
            }
        }
    }
}
