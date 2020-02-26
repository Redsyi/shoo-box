using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public int scene;
    public bool canChangeLevels = true;
    public void OnTriggerEnter(Collider other)
    {
        if (canChangeLevels && other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(scene);
        }
    }
}
