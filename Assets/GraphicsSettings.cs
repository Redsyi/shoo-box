using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [SerializeField] Text resolutionText;
    [SerializeField] Text qualityText;
    [SerializeField] Text fullscreenText;

    private string[] resolutionStrings;
    private string[] qualityStrings;
    private int currentResolution;
    private int currentFullScreenMode;
    private int currentQualityLevel;
    string currQualityName => qualityStrings[currentQualityLevel];
    string currResolutionName => resolutionStrings[currentResolution];
    string currFullscreenName => currentFullScreenMode != 0 ? "On" : "Off";

    public void LoadSettings()
    {
        if (resolutionStrings == null || resolutionStrings.Length == 0)
            resolutionStrings = PlayerData.GetResolutionNames();
        if (qualityStrings == null || qualityStrings.Length == 0)
            qualityStrings = PlayerData.GetQualityNames();
        currentResolution = PlayerData.GetGraphicSetting(GraphicsSetting.RESOLUTION_INDEX);
        currentFullScreenMode = PlayerData.GetGraphicSetting(GraphicsSetting.FULLSCREEN);
        currentQualityLevel = PlayerData.GetGraphicSetting(GraphicsSetting.QUALITY_INDEX);

        resolutionText.text = currResolutionName;
        qualityText.text = currQualityName;
        fullscreenText.text = currFullscreenName;
    }

    public void ResolutionMoveLeft()
    {
        --currentResolution;
        if (currentResolution < 0)
            currentResolution = resolutionStrings.Length - 1;

        resolutionText.text = currResolutionName;
        PlayerData.SetResolutionIndex(currentResolution);
    }

    public void ResolutionMoveRight()
    {
        ++currentResolution;
        if (currentResolution >= resolutionStrings.Length)
            currentResolution = 0;

        resolutionText.text = currResolutionName;
        PlayerData.SetResolutionIndex(currentResolution);
    }

    public void FullScreenToggle()
    {
        currentFullScreenMode = (currentFullScreenMode != 0 ? 0 : 1);

        fullscreenText.text = currFullscreenName;
        PlayerData.SetFullscreen(currentFullScreenMode != 0);
    }

    public void QualityMoveLeft()
    {
        --currentQualityLevel;
        if (currentQualityLevel < 0)
            currentQualityLevel = qualityStrings.Length - 1;

        qualityText.text = currQualityName;
        PlayerData.SetQualityIndex(currentQualityLevel);
    }

    public void QualityMoveRight()
    {
        ++currentQualityLevel;
        if (currentQualityLevel >= qualityStrings.Length)
            currentQualityLevel = 0;

        qualityText.text = currQualityName;
        PlayerData.SetQualityIndex(currentQualityLevel);
    }

    public void Apply()
    {
        PlayerData.ApplyGraphicsSettings();
    }
}
