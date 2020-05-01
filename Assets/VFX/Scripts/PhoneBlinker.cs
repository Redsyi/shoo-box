using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// occassionally activates a blink VFX after some time has passed
/// </summary>
public class PhoneBlinker : MonoBehaviour, IKickable
{
    public float timeToBlink;

    VisualEffect blinker;
    float timeSinceLastKick;
    Player player;

    void Start()
    {
        blinker = GetComponentInChildren<VisualEffect>();
        player = Player.current;
    }

    public void OnKick(GameObject kicker)
    {
        timeSinceLastKick = 0f;
        blinker.Stop();
    }
    
    void Update()
    {
        if (player.shoeManager.currShoe != ShoeType.BAREFOOT)
        {
            if (timeSinceLastKick < timeToBlink)
            {
                timeSinceLastKick += Time.deltaTime;
                if (timeSinceLastKick >= timeToBlink)
                {
                    blinker.Play();
                }
            }
        }
    }
}
