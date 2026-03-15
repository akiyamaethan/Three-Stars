using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AccessibilityFilterManager : MonoBehaviour
{
    public static AccessibilityFilterManager Instance;

    [SerializeField] private Volume globalVolume;

    private ColorAdjustments colorAdjustments;

    private const string GrayscaleKey = "Accessibility_Grayscale";
    private const string HighContrastKey = "Accessibility_HighContrast";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out colorAdjustments);
        }

        ApplySavedSettings();
    }

    public void ToggleGrayscale()
    {
        bool grayscaleOn = PlayerPrefs.GetInt(GrayscaleKey, 0) == 1;
        grayscaleOn = !grayscaleOn;

        PlayerPrefs.SetInt(GrayscaleKey, grayscaleOn ? 1 : 0);

        if (grayscaleOn)
        {
            PlayerPrefs.SetInt(HighContrastKey, 0);
        }

        PlayerPrefs.Save();
        ApplySavedSettings();
    }

    public void ToggleHighContrast()
    {
        bool highContrastOn = PlayerPrefs.GetInt(HighContrastKey, 0) == 1;
        highContrastOn = !highContrastOn;

        PlayerPrefs.SetInt(HighContrastKey, highContrastOn ? 1 : 0);

        if (highContrastOn)
        {
            PlayerPrefs.SetInt(GrayscaleKey, 0);
        }

        PlayerPrefs.Save();
        ApplySavedSettings();
    }

    public void TurnOffAllFilters()
    {
        PlayerPrefs.SetInt(GrayscaleKey, 0);
        PlayerPrefs.SetInt(HighContrastKey, 0);
        PlayerPrefs.Save();

        ApplySavedSettings();
    }

    public bool IsGrayscaleOn()
    {
        return PlayerPrefs.GetInt(GrayscaleKey, 0) == 1;
    }

    public bool IsHighContrastOn()
    {
        return PlayerPrefs.GetInt(HighContrastKey, 0) == 1;
    }

    public void ApplySavedSettings()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.active = true;

            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.contrast.overrideState = true;
            colorAdjustments.saturation.overrideState = true;
            colorAdjustments.colorFilter.overrideState = true;
            colorAdjustments.hueShift.overrideState = true;

            if (IsGrayscaleOn())
            {
                colorAdjustments.postExposure.value = 0f;
                colorAdjustments.contrast.value = 0f;
                colorAdjustments.saturation.value = -100f;
                colorAdjustments.colorFilter.value = Color.white;
                colorAdjustments.hueShift.value = 0f;
            }
            else if (IsHighContrastOn())
            {
                colorAdjustments.postExposure.value = 0.1f;
                colorAdjustments.contrast.value = 50f;
                colorAdjustments.saturation.value = -10f;
                colorAdjustments.colorFilter.value = Color.white;
                colorAdjustments.hueShift.value = 0f;
            }
            else
            {
                colorAdjustments.postExposure.value = 0f;
                colorAdjustments.contrast.value = 0f;
                colorAdjustments.saturation.value = 0f;
                colorAdjustments.colorFilter.value = Color.white;
                colorAdjustments.hueShift.value = 0f;
            }
        }

        UpdateAllCardAccessibility();
    }

    private void UpdateAllCardAccessibility()
    {
        CardAccessibility[] cards = FindObjectsOfType<CardAccessibility>(true);

        foreach (CardAccessibility card in cards)
        {
            card.ApplyAccessibility();
        }
    }
}