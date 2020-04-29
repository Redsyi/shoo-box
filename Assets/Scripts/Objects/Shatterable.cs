using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Shatterable : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rigidbody;

    [Header("Shatter")]
    public bool shatter;
    public GameObject intactModel;
    public GameObject shatteredModel;

    [Header("VFX")]
    public VisualEffect impactEffect;

    [Header("Stats")]
    public float maxY;
    public AK.Wwise.Event impactSound;

    bool hit;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hit && transform.position.y <= maxY)
        {
            hit = true;
            impactSound.Post(gameObject);
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
            rigidbody.isKinematic = true;
        }
    }
}
