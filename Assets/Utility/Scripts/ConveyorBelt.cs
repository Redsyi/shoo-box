using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// conveyor belt that moves things on top of it depending on whether the system is on or not
/// </summary>
public class ConveyorBelt : MonoBehaviour
{
    public Vector3 move;
    public float speed;
    public static bool active = true;
    private MeshRenderer renderer;
    private float beltOffset;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        active = true;
    }

    public void OnTriggerStay(Collider other)
    {
        if(active)
        {
            Player player = other.GetComponentInParent<Player>();

            if (player)
                player.transform.position += move * speed * Time.fixedDeltaTime;
            else if(other.CompareTag("Luggage"))
                other.transform.position += move * speed * Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        if (active)
        {
            beltOffset += Time.deltaTime * speed;
            renderer.material.SetFloat("_Offset", beltOffset);
        }
    }

    public static void ToggleActive()
    {
        active = !active;
    }
    
}
