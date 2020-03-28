using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerInputCheck : MonoBehaviour
{

    private UITutorialManager manager;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<UITutorialManager>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        manager.controller = player.usingController;
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
        if (player.wigglesRequired == 0)
        {
            manager.toiletFocusStealer.stealAfterTime = 4f;
        }
        manager.FinishCamera();
        //manager.ShowLegform();
    }

    public void OnAction()
    {
        manager.FinishUse();
    }





}
