using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoeManager : MonoBehaviour
{
    public ShoeType currShoe;

    private GameObject currShoeGameObject;
    public GameObject boots;
    public GameObject sandals;
    public SandalSlinger sandalSlinger;
    private UIShoeTag shoeTagUI;
    private bool[] acquiredShoes = { true, false, false, false };

    private void Awake()
    {
        shoeTagUI = FindObjectOfType<UIShoeTag>();
    }


    public void SwitchTo(ShoeType shoe)
    {
        if (acquiredShoes[(int)shoe])
        {
            if (currShoeGameObject != null)
                currShoeGameObject.SetActive(false);

            switch (shoe)
            {
                case ShoeType.BAREFOOT:
                    currShoeGameObject = null;
                    break;
                case ShoeType.BOOTS:
                    boots.SetActive(true);
                    currShoeGameObject = boots;
                    break;
                case ShoeType.FLIPFLOPS:
                    //todo activate sandals
                    sandals.SetActive(true);
                    currShoeGameObject = sandals;
                    break;
                default:
                    currShoeGameObject = null;
                    break;
            }

            currShoe = shoe;
            shoeTagUI.SwitchTo(shoe);
        } else
        {
            shoeTagUI.Wiggle();
        }
    }

    public void UseShoes()
    {
        switch (currShoe)
        {
            case ShoeType.BAREFOOT:
                break;
            case ShoeType.BOOTS:
                boots.GetComponentInChildren<BootKicker>().Kick();
                break;
            case ShoeType.FLIPFLOPS:
                sandalSlinger.Sling();
                break;
            default:
                break;
        }
    }

    public void Acquire(ShoeType shoe)
    {
        acquiredShoes[(int)shoe] = true;
    }
}
