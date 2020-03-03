using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootKicker : MonoBehaviour
{
    public Collider hitbox;
    public AudioClip kickClip;
    public ParticleSystem kickParticleSystem;

    private void OnTriggerEnter(Collider other)
    {
        bool hitSomething = false;
        foreach (IKickable kickable in other.gameObject.GetComponents<IKickable>())
        {
            hitSomething = true;
            kickable.OnKick(gameObject);
        }
        if (hitSomething)
        {
            GetComponentInParent<Player>().Rumble(RumbleStrength.MEDIUM, 0.15f);
            CameraScript.current.ShakeScreen(ShakeStrength.MEDIUM, ShakeLength.SHORT);
            kickParticleSystem.Emit(15);
            AudioManager.MakeNoise(hitbox.transform.position, 1, kickClip, 0.8f);
        }
    }

    public void Kick()
    {
        StartCoroutine(DoKick(0.3f));
    }

    private IEnumerator DoKick(float hitboxActiveTime)
    {
        hitbox.enabled = true;
        yield return new WaitForSeconds(hitboxActiveTime);
        hitbox.enabled = false;
    }
}
