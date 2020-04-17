using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Fountain : MonoBehaviour
{
    public MeshRenderer[] littleWaterPools;

    bool destroyed;
    LayerMask waterCollisionMask;
    VisualEffect waterFX;

    void Start()
    {
        waterCollisionMask = LayerMask.GetMask("Default", "Player");
        waterFX = GetComponent<VisualEffect>();
    }


    private void FixedUpdate()
    {
        if (destroyed)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.up, out hit, 5, waterCollisionMask))
            {
                waterFX.SetFloat("CollisionPlane", hit.distance / transform.localScale.y);
            } else
            {
                waterFX.SetFloat("CollisionPlane", 10);
            }
        }
    }

    public void OnKick()
    {
        if (!destroyed)
        {
            destroyed = true;
            foreach (MeshRenderer renderer in littleWaterPools)
            {
                renderer.enabled = false;
            }
            waterFX.SendEvent("OnDestroy");
        }
    }
}
