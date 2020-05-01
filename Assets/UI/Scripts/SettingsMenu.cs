using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//deprecated
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Text titleText;
    [SerializeField] CanvasGroup soundGroup;
    [SerializeField] CanvasGroup graphicsGroup;
    [SerializeField] CanvasGroup gameplayGroup;
    [SerializeField] Button backApplyButton;
    [SerializeField] Text backApplyButtonText;
    [SerializeField] GraphicsSettings graphicsSettings;

    private CanvasGroup currentGroup;

    private void Start()
    {
        currentGroup = soundGroup;
        ApplyToBack();
    }

    public void BackToApply()
    {
        backApplyButton.onClick.AddListener(graphicsSettings.Apply);
        backApplyButtonText.text = "Apply";
    }

    public void ApplyToBack()
    {
        backApplyButton.onClick.RemoveListener(graphicsSettings.Apply);
        backApplyButtonText.text = "Back";
    }

    public void SwitchToGraphics()
    {
        SwitchGroup(graphicsGroup, "Graphics Settings");
    }

    public void SwitchToSound()
    {
        SwitchGroup(soundGroup, "Sound Settings");
    }

    public void SwitchToGameplay()
    {
        SwitchGroup(gameplayGroup, "Game Settings");
    }

    private void SwitchGroup(CanvasGroup target, string newTitle)
    {
        target.alpha = 1f;
        target.interactable = true;
        currentGroup.alpha = 0f;
        currentGroup.interactable = false;
        currentGroup = target;
        titleText.text = newTitle;
    }
}
