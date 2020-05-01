using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that manages the player's current shoe loadout
/// </summary>
public class PlayerShoeManager : MonoBehaviour
{
    //currently equipped shoe
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

    //attempts to switch to the specified shoe type and animates the shoe tag in the UI accordingly
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

    //uses the currently equipped pair of shoes
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

    //gives access to the specified shoe type
    public void Acquire(ShoeType shoe)
    {
        acquiredShoes[(int)shoe] = true;
    }

    /// <summary>
    /// does the player own this type of shoes?
    /// </summary>
    public bool Has(ShoeType shoe)
    {
        return acquiredShoes[(int)shoe];
    }
}
