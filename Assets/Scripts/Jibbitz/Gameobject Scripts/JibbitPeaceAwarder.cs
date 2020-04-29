using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitPeaceAwarder : MonoBehaviour
{
    public Jibbit jibbit;
    bool given;

    private void OnTriggerEnter(Collider other)
    {
        if (!given && CityDirector.current && CityDirector.current.intensity == 0)
        {
            given = true;
            JibbitManager.AcquireJibbit(jibbit.id);
            JibbitAcquiredPopup.current.Acquire(jibbit);
        }
    }
}
