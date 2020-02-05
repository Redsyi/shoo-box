using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kicker : MonoBehaviour
{
    public Collider hitbox;

    private void Start()
    {
        hitbox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        IKickable kickable = other.gameObject.GetComponent<IKickable>();
        if (kickable != null)
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
        yield return new WaitForSeconds(hitboxActiveTime);
        hitbox.enabled = false;
    }
}
