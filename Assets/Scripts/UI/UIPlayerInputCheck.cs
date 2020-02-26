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
        manager.ShowCamera();
    }

    public void OnRotate()
    {
        manager.ShowLegform();
    }

    public void OnChangeForm()
    {
        manager.ShowInteract();
    }

    public void OnAction()
    {
        manager.FinishUse();
    }





}
