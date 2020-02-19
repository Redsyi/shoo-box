using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{

    public int scene;
    public void OnTriggerEnter(Collider other)
    {
        print(other.transform.parent.gameObject.name);

        if (other.transform.parent.gameObject.name == "Player")
        {
            SceneManager.LoadScene(scene);
        }
    }
}
