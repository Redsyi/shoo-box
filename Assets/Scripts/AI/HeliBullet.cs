using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliBullet : MonoBehaviour
{
    public float trailLifetime;
    public LineRenderer renderer;

    public void Fire()
    {
        StartCoroutine(DoFire());
    }

    IEnumerator DoFire()
    {
        RaycastHit raycastHit;
        renderer.SetPosition(0, transform.position);
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, 40, LayerMask.GetMask("Player", "Obstructions")))
        {
            renderer.SetPosition(1, raycastHit.point);
        } else
        {
            renderer.SetPosition(1, transform.position + transform.forward * 40);
        }
        renderer.enabled = true;
        yield return new WaitForSeconds(trailLifetime);
        Destroy(gameObject);
    }
}
