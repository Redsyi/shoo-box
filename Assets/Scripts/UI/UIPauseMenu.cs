using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Event = AK.Wwise.Event;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    public bool paused { get; private set; }
    public static UIPauseMenu instance;
    public GameObject defaultSelected;
    public GameObject backButton;
    public GameObject settingsDefaultButton;
    public Event pauseSound;
    public Event unpauseSound;
    public Event onPressed;
    public Event onHovered;
    public CanvasGroup menu;
    public CanvasGroup controls;
    public CanvasGroup settings;

    public UIButtonPane buttonPane;
    public GameObject[] panes;
    public int currPaneIdx;

    GameObject currPane => panes[currPaneIdx];
    
    //Toggle controls view between controller/keyboard
    public GameObject toggleController;
    public GameObject toggleKeyboard;
    public GameObject toggleLabel;
    private string toggleText;
    public bool conToggled { get; private set; }

    private void Start()
    {
        instance = this;
        gameObject.SetActive(false);
        conToggled = false;
    }

    
    public void TogglePause()
    {
        paused = !paused;

        if (paused)
            pauseSound.Post(gameObject);
        else
            unpauseSound.Post(gameObject);

        if (paused)
            buttonPane.Activate();
        else
            buttonPane.Deactivate();

        gameObject.SetActive(paused);

        if (paused)
            EventSystem.current.SetSelectedGameObject(defaultSelected);

        
        Time.timeScale = (paused ? 0 : 1);
        (paused ? pauseSound : unpauseSound)?.Post(gameObject);

    }

    public void NextTab()
    {
        if (currPaneIdx < panes.Length - 1)
        {
            AnimateOut(currPane, UIDirection.LEFT);
            currPaneIdx++;
            AnimateIn(currPane, UIDirection.RIGHT);
            buttonPane.Select(currPaneIdx);
        }
    }

    public void PrevTab()
    {
        if (currPaneIdx > 0)
        {
            AnimateOut(currPane, UIDirection.RIGHT);
            currPaneIdx--;
            AnimateIn(currPane, UIDirection.LEFT);
            buttonPane.Select(currPaneIdx);
        }
    }


    public void OnRetryPressed()
    {
        onHovered.Post(gameObject);
        Time.timeScale = 1;
        LevelBridge.Reload("Gave up so easily?");
        onPressed.Post(gameObject);
    }

    public void OnExitPressed()
    {
        onHovered.Post(gameObject);
        Time.timeScale = 1;
        LevelBridge.BridgeTo("MainMenu", "See ya later!");
        onPressed.Post(gameObject);
    }

    public void OnControlsPressed()
    {
        /*displayControls = !displayControls;
         menu.alpha = (displayControls ? 0 : 1);
         menu.interactable = displayControls;
         controls.alpha = (displayControls ? 1 : 0);
         controls.interactable = displayControls;*/
        EventSystem.current.SetSelectedGameObject(backButton);
        menu.alpha = 0f;
        menu.interactable = false;
        controls.alpha = 1f;
        controls.interactable = true;
    }

    public void OnSettingsPressed()
    {
        /*displayControls = !displayControls;
         menu.alpha = (displayControls ? 0 : 1);
         menu.interactable = displayControls;
         controls.alpha = (displayControls ? 1 : 0);
         controls.interactable = displayControls;*/
        EventSystem.current.SetSelectedGameObject(settingsDefaultButton);
        menu.alpha = 0f;
        menu.interactable = false;
        settings.alpha = 1f;
        settings.interactable = true;
    }

    public void OnBackPressed()
    {
        /*displayControls = !displayControls;
        menu.alpha = (displayControls ? 0 : 1);
        menu.interactable = displayControls;
        controls.alpha = (displayControls ? 1 : 0);*/
        EventSystem.current.SetSelectedGameObject(defaultSelected);
        menu.alpha = 1f;
        menu.interactable = true;
        controls.alpha = 0f;
        controls.interactable = false;
    }

    public void OnToggleInputPressed()
    {
        /*Feature request: toggle detects which input system is used
         and automatically switches to the one in use
         when menu is first opened*/
        conToggled = !conToggled;
        toggleKeyboard.SetActive(!conToggled);
        toggleController.SetActive(conToggled);
        onPressed.Post(gameObject);

        if (conToggled)
        {
            toggleText = "Controller";
            toggleLabel.GetComponent<Text>().text = toggleText;
        }

        if (conToggled == false)
        {
            toggleText = "Keyboard";
            toggleLabel.GetComponent<Text>().text = toggleText;
        }
    }

    void AnimateIn(GameObject pane, UIDirection direction)
    {
        StartCoroutine(DoAnimateIn(pane, direction));
    }

    void AnimateOut(GameObject pane, UIDirection direction)
    {
        StartCoroutine(DoAnimateOut(pane, direction));
    }

    IEnumerator DoAnimateIn(GameObject pane, UIDirection direction)
    {
        float animationTime = 0.5f;
        float timePassed = 0f;
        float progress = 0f;
        RectTransform rect = pane.GetComponent<RectTransform>();
        pane.SetActive(true);
        Vector2 originalPivot = (direction == UIDirection.LEFT ? new Vector2(1, 0.5f) : new Vector2(0, 0.5f));
        Vector2 originalAnchor = (direction == UIDirection.LEFT ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f));
        Vector2 destPivot = new Vector2(0.5f, 0.5f);
        while (progress < 1)
        {
            yield return null;
            timePassed += Time.unscaledDeltaTime;
            progress = Mathf.Clamp01(timePassed / animationTime);
            rect.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            rect.anchoredPosition = Vector2.Lerp(originalAnchor, destPivot, progress);
            rect.localPosition = Vector3.zero;
        }
    }

    IEnumerator DoAnimateOut(GameObject pane, UIDirection direction)
    {
        float animationTime = 0.5f;
        float timePassed = 0f;
        float progress = 0f;
        RectTransform rect = pane.GetComponent<RectTransform>();
        Vector2 destPivot = (direction == UIDirection.LEFT ? new Vector2(1, 0.5f) : new Vector2(0, 0.5f));
        Vector2 destAnchor = (direction == UIDirection.LEFT ? new Vector2(0, 0.5f) : new Vector2(1, 0.5f));
        Vector2 originalPivot = new Vector2(0.5f, 0.5f);
        while (progress < 1)
        {
            yield return null;
            timePassed += Time.unscaledDeltaTime;
            progress = Mathf.Clamp01(timePassed / animationTime);
            rect.pivot = Vector2.Lerp(originalPivot, destPivot, progress);
            rect.anchoredPosition = Vector2.Lerp(originalPivot, destAnchor, progress);
            rect.localPosition = Vector3.zero;
        }
        pane.SetActive(false);
    }
}
