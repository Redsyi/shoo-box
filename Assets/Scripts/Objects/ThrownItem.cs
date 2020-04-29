using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
            if (shatter)
            {
                intactModel.SetActive(false);
                shatteredModel.SetActive(true);
                foreach (Rigidbody pieceRB in shatteredModel.GetComponentsInChildren<Rigidbody>())
                {
                    pieceRB.AddExplosionForce(10, transform.position, 1);
                }
            }
            if (impactEffect)
            {
                impactEffect.Play();
            }

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
