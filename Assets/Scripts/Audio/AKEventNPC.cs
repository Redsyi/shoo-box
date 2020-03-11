using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = AK.Wwise.Event;

public class AKEventNPC : MonoBehaviour
{
    public Event investigateSound;
    public Event somethingWrongSound;
    public Event playerSpottedSound;
    public Event playerCaughtSound;
    public Event fixingSound;
    public Event giveUpSound;

    public void StartedInvestigation()
    {
        investigateSound?.Post(gameObject);
    }
    public void SomethingWrong()
    {
        somethingWrongSound?.Post(gameObject);
    }
    public void PlayerSpotted()
    {
        playerSpottedSound?.Post(gameObject);
    }
    public void PlayerCaught()
    {
        playerCaughtSound?.Post(gameObject);
    }
    public void Fixing()
    {
        fixingSound?.Post(gameObject);
    }
    public void GiveUp()
    {
        giveUpSound?.Post(gameObject);
    }
}
