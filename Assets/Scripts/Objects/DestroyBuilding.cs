using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBuilding : MonoBehaviour, IKickable
{
    public int maxKicks;
    public float toGround;

    private float originalY;
    private float newY;
    private int numKicks = 0;
   
    public void OnKick(GameObject kicker)
    {
        numKicks++;
        if (numKicks < maxKicks)
        {
            newY -= (originalY / maxKicks);
            print("Original: " + originalY);
            print("New: " + newY);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else if (numKicks == maxKicks)
            transform.position = new Vector3(transform.position.x, toGround, transform.position.z);
        

    }

    private void Start()
    {
        originalY = transform.position.y;
        newY = originalY;
    }

}
