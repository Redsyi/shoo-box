using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class managing the player's boots that kick shit
/// </summary>
public class BootKicker : MonoBehaviour
{
    public Collider hitbox;
    public ParticleSystem kickParticleSystem;

    HashSet<IKickable> alreadyKicked;

    private void OnTriggerEnter(Collider other)
    {
        bool hitSomething = false;
        foreach (IKickable kickable in other.gameObject.GetComponents<IKickable>())
        {
            if (!alreadyKicked.Contains(kickable))
            {
                hitSomething = true;
                kickable.OnKick(gameObject);
                alreadyKicked.Add(kickable);
            }
        }
        if (hitSomething)
        {
            GetComponentInParent<Player>().Rumble(RumbleStrength.MEDIUM, 0.15f);
            CameraScript.current.ShakeScreen(ShakeStrength.MEDIUM, ShakeLength.SHORT);
            kickParticleSystem.Emit(15);
            AudioManager.MakeNoise(hitbox.transform.position, 1, null, 0.8f);
        }
    }

    public void Kick()
    {
        StartCoroutine(DoKick(0.3f));
    }

    private IEnumerator DoKick(float hitboxActiveTime)
    {
        if (alreadyKicked == null)
        {
            alreadyKicked = new HashSet<IKickable>();
        }
        hitbox.enabled = true;
        yield return new WaitForSeconds(hitboxActiveTime);
        hitbox.enabled = false;
        alreadyKicked.Clear();
    }
}
