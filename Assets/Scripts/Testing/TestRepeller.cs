using System.Collections;
using UnityEngine;

public class TestRepeller : MonoBehaviour
{
    private bool repelling;
    public MeshRenderer renderer;
    public float repelActiveTime;

    public void Repel()
    {
        if (!repelling)
            StartCoroutine(DoRepel(repelActiveTime));
    }

    IEnumerator DoRepel(float seconds)
    {
        repelling = true;
        renderer.enabled = true;
        yield return new WaitForSeconds(seconds);
        renderer.enabled = false;
        repelling = false;
    }
}
