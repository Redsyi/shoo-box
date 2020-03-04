using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Vector3 move;
    public float speed;
    public static bool active = true;

    public void OnTriggerStay(Collider other)
    {
        if(active)
        {
            Player player = other.GetComponentInParent<Player>();

            if(player)
                player.transform.position += move * speed * Time.fixedDeltaTime;
        }
    }

    public static void ToggleActive()
    {
        active = !active;

        foreach (ConveyorBelt conveyorBelt in FindObjectsOfType<ConveyorBelt>())
            conveyorBelt.GetComponent<MeshRenderer>().material.SetFloat("_MoveSpeed", (active ? 1 : 0));
    }
    
}
