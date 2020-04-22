using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    [SerializeField] Text fullScreenText;
    [SerializeField] Text qualityText;
    [SerializeField] Text resolutionText;
    [SerializeField] List<Vector2> resolutionValues;
    [SerializeField] List<FullScreenMode> fullScreenModes;
    [SerializeField] UnityEvent onValueChanged;
    [SerializeField] UnityEvent onValueReverted;

    private List<string> resolutionStrings;
    private string[] qualityStrings;
    private int currentResolution;
    private int currentFullScreenMode;
    private int currentQualityLevel;
    private int initialResolution;
    private int initialFullScreenMode;
    private int initialQualityLevel;
    private bool dirty;


    // Start is called before the first frame update
    void Start()
    {
        resolutionStrings = new List<string>();
        int i = 0;
        foreach(var resolutionValue in resolutionValues)
        {
            resolutionStrings.Add(resolutionValue.x + "x" + resolutionValue.y);
            if (resolutionValue.x == Screen.width && resolutionValue.y == Screen.height)
            {
                currentResolution = i;
                initialResolution = currentResolution;
            }
            i++;
        }
        resolutionText.text = resolutionStrings[currentResolution];

        i = 0;
        foreach (var mode in fullScreenModes)
        {
            if (Screen.fullScreenMode == mode)
            {
                currentFullScreenMode = i;
                initialFullScreenMode = currentFullScreenMode;
            }
            i++;
        }
        fullScreenText.text = fullScreenModes[currentFullScreenMode].ToString();

        i = 0;
        qualityStrings = QualitySettings.names;
        foreach (var qualityName in qualityStrings)
        {
            if (QualitySettings.GetQualityLevel() == i)
            {
                currentQualityLevel = i;
                initialQualityLevel = currentQualityLevel;
            }
            i++;
        }
        qualityText.text = qualityStrings[currentQualityLevel];
    }

    public void ResolutionMoveLeft()
    {
        currentResolution = currentResolution == 0 ? resolutionValues.Count - 1 : currentResolution - 1;
        resolutionText.text = resolutionStrings[currentResolution];
        CheckDirty();

    }

    public void ResolutionMoveRight()
    {
        currentResolution = currentResolution == resolutionValues.Count - 1 ? 0 : currentResolution + 1;
        resolutionText.text = resolutionStrings[currentResolution];
        CheckDirty();

    }
    public void FullScreenMoveLeft()
    {
        currentFullScreenMode = currentFullScreenMode == 0 ? fullScreenModes.Count - 1 : currentFullScreenMode - 1;
        fullScreenText.text = fullScreenModes[currentFullScreenMode].ToString();
        CheckDirty();

    }

    public void FullScreenMoveRight()
    {
        currentFullScreenMode = currentFullScreenMode == fullScreenModes.Count - 1 ? 0 : currentFullScreenMode + 1;
        fullScreenText.text = fullScreenModes[currentFullScreenMode].ToString();
        CheckDirty();

    }

    public void QualityMoveLeft()
    {
        currentQualityLevel = currentQualityLevel == 0 ? qualityStrings.Length - 1 : currentQualityLevel - 1;
        qualityText.text = qualityStrings[currentQualityLevel];
        CheckDirty();

    }

    public void QualityMoveRight()
    {
        currentQualityLevel = currentQualityLevel == qualityStrings.Length - 1 ? 0 : currentQualityLevel + 1;
        qualityText.text = qualityStrings[currentQualityLevel];
        CheckDirty();
    }

    private void CheckDirty()
    {
        if (dirty && initialQualityLevel == currentQualityLevel && initialFullScreenMode == currentFullScreenMode && initialResolution == currentFullScreenMode)
        {
            dirty = !dirty;
            onValueChanged.Invoke();
        }
        if (!dirty && !(initialQualityLevel == currentQualityLevel && initialFullScreenMode == currentFullScreenMode && initialResolution == currentFullScreenMode))
        {
            dirty = !dirty;
            onValueReverted.Invoke();
        }
    }

    public void Apply()
    {
        Screen.SetResolution((int)resolutionValues[currentResolution].x, (int)resolutionValues[currentResolution].y, fullScreenModes[currentFullScreenMode]);
        QualitySettings.SetQualityLevel(currentQualityLevel);
        initialFullScreenMode = currentFullScreenMode;
        initialQualityLevel = currentQualityLevel;
        initialResolution = currentResolution;
        CheckDirty();
    }
}
