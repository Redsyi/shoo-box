using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneAfterSeconds : MonoBehaviour
{
    public int destinationScene;
    public float afterSeconds;
    public Animator transition;
    public Animation leftShoeStep;
    public bool isCutscene;

    // Start is called before the first frame update
    void Start()
    {
        if (isCutscene)
        {
            Invoke("ChangeScene", afterSeconds);
        }
    }

    void ChangeScene()
    {
        StartCoroutine(TransitionWipe());
        //UnityEngine.SceneManagement.SceneManager.LoadScene(destinationScene);
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

    IEnumerator TransitionWipe()
    {
        transition.SetTrigger("CutsceneStart");
        leftShoeStep.Play();
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(destinationScene);
    }
}
