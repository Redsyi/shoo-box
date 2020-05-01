using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private MeshRenderer renderer;
    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        renderer.material.SetFloat("_Alpha", 0.8f);
    }
    private void OnTriggerExit(Collider other)
    {
        renderer.material.SetFloat("_Alpha", 0f);
    }
}
