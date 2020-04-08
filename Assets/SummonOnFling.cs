using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonOnFling : MonoBehaviour
{
    public AIInterest[] AIs;
    public GameObject to;
    public float investigateTime = 3f;

    private void Start()
    {
        if (AIs.Length == 0)
            Debug.LogWarning("SummonOnKick component will never summon any AIs");
        if (to == null)
            Debug.LogError("SummonOnKick component has no target to summon AIs to");
    }

    public void OnFling()
    {
        AIAgent.SummonAI(to, investigateTime, AIs);
    }
}
