using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        print(other.transform.parent.gameObject.name);

        if (other.transform.parent.gameObject.name == "Player")
        {
            //Transform leftDoor = other.transform.parent.GetChild(2);
            //Transform rightDoor = other.transform.parent.GetChild(3);
            //leftDoor.position = new Vector3(leftDoor.position.x, leftDoor.position.y, -2.61f);
            //rightDoor.position = new Vector3(rightDoor.position.x, rightDoor.position.y, 2.29f);
            SceneManager.LoadScene(1);
        }
    }
}
