using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoeManager : MonoBehaviour
{
    public ShoeType currShoe;

    private GameObject currShoeGameObject;
    public GameObject boots;

    public void SwitchTo(ShoeType shoe)
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
            default:
                currShoeGameObject = null;
                break;
        }

        currShoe = shoe;
    }

    public void UseShoes()
    {
        switch (currShoe)
        {
            case ShoeType.BAREFOOT:
                break;
            case ShoeType.BOOTS:
                boots.GetComponent<BootKicker>().Kick();
                break;
            default:
                break;
        }
    }
}
