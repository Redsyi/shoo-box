using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentKickable : MonoBehaviour, IKickable
{
    [SerializeField] private TestKickable[] children;

    // Update is called once per frame
    public void OnKick(GameObject kicker)
    {
        foreach (TestKickable kickable in children)
        {
            kickable.OnKick(kicker);
        }
    }
}
