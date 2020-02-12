using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButton : MonoBehaviour, IKickable
{
    public Animator animator;
    public Canvas labelCanvas;
    private Camera camera;

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    public void OnKick(GameObject kicker)
    {
        animator.SetTrigger("Press");
    }

    private void LateUpdate()
    {
        labelCanvas.transform.localEulerAngles = new Vector3(0, camera.transform.eulerAngles.y);
    }
}
