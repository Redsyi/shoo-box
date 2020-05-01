using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// class that represents an item thrown by npcs
/// </summary>
public class ThrownItem : MonoBehaviour
{
    public int type;
    [HideInInspector] public GameObject thrower;

    [Header("Components")]
    public Rigidbody rigidbody;

    [Header("Shatter")]
    public bool shatter;
    public GameObject intactModel;
    public GameObject shatteredModel;

    [Header("VFX")]
    public VisualEffect impactEffect;

    bool hit;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hit)
        {
            hit = true;

            //activate fractured model and rigidbodies, deactive intact model
            if (shatter)
            {
                intactModel.SetActive(false);
                shatteredModel.SetActive(true);
                foreach (Rigidbody pieceRB in shatteredModel.GetComponentsInChildren<Rigidbody>())
                {
                    pieceRB.AddExplosionForce(10, transform.position, 1);
                }
            }

            //play vfx
            if (impactEffect)
            {
                impactEffect.Play();
            }

            //see if jibbit should be awarded
            if (!JibbitHotdogGiver.given && collision.gameObject.CompareTag("Player"))
            {
                JibbitHotdogGiver jibGiver = thrower.GetComponent<JibbitHotdogGiver>();
                if (jibGiver)
                {
                    jibGiver.NotifyHit(type);
                }
            }
        }
    }
}
