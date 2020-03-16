using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneAfterSeconds : MonoBehaviour
{
    public int destinationScene;
    public float afterSeconds;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ChangeScene", afterSeconds);
    }

    void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(destinationScene);
    }

    public void Skip()
    {
        CancelInvoke("ChangeScene");
        ChangeScene();
    }

    public void OnSkip()
    {
        Skip();
    }
}
