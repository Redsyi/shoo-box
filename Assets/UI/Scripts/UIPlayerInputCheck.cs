using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages some player input during the tutorial
/// </summary>
public class UIPlayerInputCheck : MonoBehaviour
{

    private UITutorialManager manager;
    private Player player;
    bool didRotation;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<UITutorialManager>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        manager.controller = Controls.usingController;
        if (player.shoeManager.currShoe == ShoeType.BOOTS)
            manager.ShowUse();
    }

    public void OnMove()
    {
        if (player.wigglesRequired == 0)
        {
            manager.ShowCamera();
        }
    }

    public void OnRotate()
    {
        if (player.wigglesRequired == 0 && !didRotation)
        {
            manager.toiletFocusStealer.stealAfterTime = 4f;
            manager.FinishCamera();
            didRotation = true;
        }
    }

    public void OnAction()
    {
        manager.FinishUse();
    }





}
