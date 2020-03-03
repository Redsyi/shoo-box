using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandalSlinger : MonoBehaviour
{
    public SandalProjectile sandalPrefab;

    public void Sling()
    {
        SandalProjectile sandal = Instantiate(sandalPrefab, transform.position, transform.rotation);
        sandal.slinger = this;
    }
}
