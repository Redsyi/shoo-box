using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// script to manage the main menu
/// </summary>
public class UIMainMenu : MonoBehaviour
{
    [System.Serializable]
    public class CameraInfo
    {
        public Vector3 localPosition;
        public Vector3 localRotation;
    }

    public bool options { get; private set; }
    public Animator shoebox;
    [Header("Main Buttons")]
    public RectTransform mainButtons;
    public GameObject playButton;
    public GameObject jibbzButton;
    public GameObject optionsButton;
    public Text playButtonText;
    [Header("Play Group")]
    public CanvasGroup playGroup;
    public GameObject playGroupDefaultSelected;
    [Header("Options Group")]
    public CanvasGroup optionsGroup;
    public GameObject optionsInitial;
    public UIPaneManager optionsPanes;
    [Header("Jibbitz Group")]
    public CanvasGroup jibbzGroup;
    public GameObject jibbzInitial;
    public Text jibbitNameLabel;
    public Text jibbitDescriptionLabel;
    public RectTransform jibbitLabel;

    [Header("Camera Info")]
    public Camera camera;
    public CameraInfo defaultPos;
    public CameraInfo playPos;
    public CameraInfo settingsPos;
    public CameraInfo jibbzPos;
    public float transitionTime;

    int activeAnimators;
    bool animating => activeAnimators > 0;
    bool jibbitLabelEnabled;
    MainMenuState state;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(playButton);
        StartCoroutine(OccassionallyWiggle());
        Player.prevState = null;

        PlayerData.CheckLoadedData();
        if (PlayerData.currCheckpoint != 0 || PlayerData.currLevel != PlayerData.defaultLevel.saveID)
        {
            playButtonText.text = "Continue";
        }
    }

    /// <summary>
    /// level select button pressed
    /// </summary>
    public void PlayButton()
    {
        if (!animating && state != MainMenuState.PLAY)
        {
            playGroup.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(playGroupDefaultSelected);
            StartCoroutine(AnimateCamera(defaultPos, playPos));
            StartCoroutine(AnimateMainButtons(false));
            StartCoroutine(AnimateCanvasGroup(playGroup, true));
            state = MainMenuState.PLAY;
        }
    }

    /// <summary>
    /// continue/new game button pressed
    /// </summary>
    public void ContinueButton()
    {
        if (!animating && state == MainMenuState.MAIN)
        {
            PlayerData.CheckLoadedData();
            CheckpointManager.currCheckpoint = PlayerData.currCheckpoint;
            Level destLevel = PlayerData.levels[PlayerData.currLevel];
            string levelName = (PlayerData.currCheckpoint == 0 ? destLevel.cutsceneBuildName : destLevel.levelBuildName);
            string flavorText = (PlayerData.currCheckpoint == 0 ? destLevel.cutsceneFlavorText : destLevel.levelFlavorText);
            LevelBridge.BridgeTo(levelName, flavorText);
        }
    }

    /// <summary>
    /// back button pressed, from any context
    /// </summary>
    public void BackButton()
    {
        if (!animating && state != MainMenuState.MAIN)
        {
            StartCoroutine(AnimateMainButtons(true));

            switch (state)
            {
                case MainMenuState.PLAY:
                    StartCoroutine(AnimateCamera(playPos, defaultPos));
                    StartCoroutine(AnimateCanvasGroup(playGroup, false));
                    StartCoroutine(DeactivateDelayed(playGroup.gameObject));
                    EventSystem.current.SetSelectedGameObject(playButton);
                    break;
                case MainMenuState.JIBBZ:
                    StartCoroutine(AnimateCamera(jibbzPos, defaultPos));
                    StartCoroutine(AnimateCanvasGroup(jibbzGroup, false));
                    StartCoroutine(DeactivateDelayed(jibbzGroup.gameObject));
                    EventSystem.current.SetSelectedGameObject(jibbzButton);
                    shoebox.SetBool("Open", false);
                    break;
                case MainMenuState.OPTIONS:
                    StartCoroutine(AnimateCamera(settingsPos, defaultPos));
                    StartCoroutine(AnimateCanvasGroup(optionsGroup, false));
                    StartCoroutine(DeactivateDelayed(optionsGroup.gameObject));
                    StartCoroutine(SetSelectedDelayed(optionsButton, 0.1f));
                    optionsPanes.Disappear();
                    break;
            }

            state = MainMenuState.MAIN;
        }
    }

    /// <summary>
    /// sets a new selected gameobject for the eventsystem, after a delay
    /// </summary>
    IEnumerator SetSelectedDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        EventSystem.current.SetSelectedGameObject(obj);
    }

    //Lerps the camera between two positions
    IEnumerator AnimateCamera(CameraInfo from, CameraInfo to)
    {
        float timePassed = 0f;
        activeAnimators++;

        while (timePassed < transitionTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float progress = Mathf.Clamp01(timePassed / transitionTime);
            camera.transform.localPosition = Vector3.Lerp(from.localPosition, to.localPosition, progress);
            camera.transform.localEulerAngles = Vector3.Lerp(from.localRotation, to.localRotation, progress);
        }
        activeAnimators--;
    }

    /// <summary>
    /// shows the current jibbit label with the given name and description
    /// </summary>
    public void ShowJibbitLabel(string name, string description)
    {
        if (!jibbitLabelEnabled)
        {
            StartCoroutine(AnimateJibbitLabel(true));
            jibbitLabelEnabled = true;
        }
        jibbitNameLabel.text = name;
        jibbitDescriptionLabel.text = description;
    }

    /// <summary>
    /// Hides the current jibbit label if it is active
    /// </summary>
    public void HideJibbitLabel()
    {
        if (jibbitLabelEnabled)
        {
            jibbitLabelEnabled = false;
            StartCoroutine(AnimateJibbitLabel(false));
        }
    }

    //Lerps the main button panel in or out
    IEnumerator AnimateMainButtons(bool onScreen)
    {
        float timePassed = 0f;
        activeAnimators++;

        if (onScreen)
            mainButtons.gameObject.SetActive(true);

        Vector2 originalPivot = (onScreen ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f));
        Vector2 destPivot = (onScreen ? new Vector2(1, 0.5f) : new Vector2(0, 0.5f));

        while (timePassed < transitionTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float progress = Mathf.Clamp01(timePassed / transitionTime);
            mainButtons.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            mainButtons.anchoredPosition = Vector2.zero;
        }


        if (!onScreen)
            mainButtons.gameObject.SetActive(false);
        activeAnimators--;
    }

    //Lerps the jibbit label in or out
    IEnumerator AnimateJibbitLabel(bool onScreen)
    {
        float timePassed = 0f;

        Vector2 originalPivot = (onScreen ? new Vector2(0.5f, 0) : new Vector2(0.5f, 1));
        Vector2 destPivot = (onScreen ? new Vector2(0.5f, 1) : new Vector2(0.5f, 0));

        while (timePassed < 0.15f)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float progress = Mathf.Clamp01(timePassed / 0.15f);
            jibbitLabel.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            jibbitLabel.anchoredPosition = Vector2.zero;
        }
    }

    //Lerps a canvas group to visible/invisible
    IEnumerator AnimateCanvasGroup(CanvasGroup group, bool activate)
    {
        float timePassed = 0f;
        activeAnimators++;

        while (timePassed < transitionTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float progress = Mathf.Clamp01(timePassed / transitionTime);
            group.alpha = (activate ? progress : (1 - progress));
        }
        activeAnimators--;
    }

    /// <summary>
    /// deactivates the passed in gameobjects, after a delay
    /// </summary>
    IEnumerator DeactivateDelayed(params GameObject[] objects)
    {
        yield return new WaitForSeconds(transitionTime);
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// options button selected
    /// </summary>
    public void OptionsButton()
    {
        if (!animating && state != MainMenuState.OPTIONS)
        {
            optionsGroup.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(optionsInitial);
            StartCoroutine(AnimateCamera(defaultPos, settingsPos));
            StartCoroutine(AnimateMainButtons(false));
            StartCoroutine(AnimateCanvasGroup(optionsGroup, true));
            optionsPanes.Appear();
            state = MainMenuState.OPTIONS;
        }
    }

    /// <summary>
    /// collection button selected
    /// </summary>
    public void JibbzButton()
    {
        if (!animating && state != MainMenuState.JIBBZ)
        {
            shoebox.SetBool("Open", true);
            jibbzGroup.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(jibbzInitial);
            StartCoroutine(AnimateCamera(defaultPos, jibbzPos));
            StartCoroutine(AnimateMainButtons(false));
            StartCoroutine(AnimateCanvasGroup(jibbzGroup, true));
            state = MainMenuState.JIBBZ;
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void OnCancel()
    {
        BackButton();
    }

    /// <summary>
    /// switches panes in current pane manager (currently just options)
    /// </summary>
    public void OnChangeTab(UnityEngine.InputSystem.InputValue value)
    {
        if (state == MainMenuState.OPTIONS)
        {
            if (value.Get<float>() > 0)
            {
                optionsPanes.NextPane();
            }
            else
            {
                optionsPanes.PreviousPane();
            }
        }
    }

    /// <summary>
    /// tell the box to wiggle every once in a while while in the root menu
    /// </summary>
    IEnumerator OccassionallyWiggle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5.5f, 10f));
            if (state == MainMenuState.MAIN)
            {
                shoebox.SetTrigger("Wiggle");
            }
        }
    }
}
