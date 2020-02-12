using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootKicker : MonoBehaviour
{
    public Collider hitbox;
    public AudioClip kickClip;

    private void OnTriggerEnter(Collider other)
    {
        foreach (IKickable kickable in other.gameObject.GetComponents<IKickable>())
        {
            kickable.OnKick(gameObject);
            GetComponentInParent<Player>().Rumble(RumbleStrength.MEDIUM, 0.15f);
        }
    }

    public void Kick()
    {
        StartCoroutine(DoKick(0.3f));
    }

    private IEnumerator DoKick(float hitboxActiveTime)
    {
        hitbox.enabled = true;
        AudioManager.MakeNoise(hitbox.transform.position, 1, kickClip, 0.8f);
        yield return new WaitForSeconds(hitboxActiveTime);
        hitbox.enabled = false;
    }
}
