using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandalSlinger : MonoBehaviour
{
    public SandalProjectile sandalPrefab;
    public bool slinging;

    public void Sling()
    {
        if (!slinging)
        {
            SandalProjectile sandal = Instantiate(sandalPrefab, transform.position, transform.rotation);
            sandal.slinger = this;
            slinging = true;
        }
    }
}
