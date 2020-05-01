using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to play a Wwise event when this object is kicked
/// </summary>
public class AkEventOnKick : MonoBehaviour, IKickable
{
	public AK.Wwise.Event KickSound;
    	public void OnKick(GameObject kicker)
    {
    		KickSound.Post(gameObject);
    }
}
