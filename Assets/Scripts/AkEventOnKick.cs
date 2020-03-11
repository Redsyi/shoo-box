using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AkEventOnKick : MonoBehaviour, IKickable
{
	public AK.Wwise.Event KickSound;
    	public void OnKick(GameObject kicker)
    {
    		KickSound.Post(gameObject);
    }
}
