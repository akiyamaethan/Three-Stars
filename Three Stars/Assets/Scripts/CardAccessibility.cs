using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardAccessibility : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image cardBackground;
    [SerializeField] private Image rankImage;
    [SerializeField] private Image suitImage;
    [SerializeField] private TMP_Text cardText;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text suitText;
    [SerializeField] private Outline outline;

    private Color originalBackgroundColor;
    private Color originalRankImageColor;
    private Color originalSuitImageColor;
    private Color originalCardTextColor;
    private Color originalRankTextColor;
    private Color originalSuitTextColor;

    private bool hasCachedOriginals = false;

    private float ToGray(Color c)
    {
        return (c.r + c.g + c.b) / 3f;
    }

    public void CacheOriginalVisuals()
    {
        if (cardBackground != null)
        {
            originalBackgroundColor = cardBackground.color;
        }

        if (rankImage != null)
        {
            originalRankImageColor = rankImage.color;
        }

        if (suitImage != null)
        {
            originalSuitImageColor = suitImage.color;
        }

        if (cardText != null)
        {
            originalCardTextColor = cardText.color;
        }

        if (rankText != null)
        {
            originalRankTextColor = rankText.color;
        }

        if (suitText != null)
        {
            originalSuitTextColor = suitText.color;
        }

        hasCachedOriginals = true;
    }

    public void ApplyAccessibility()
    {
        if (!hasCachedOriginals)
        {
            CacheOriginalVisuals();
        }

        if (AccessibilityFilterManager.Instance == null)
        {
            ApplyNormal();
            return;
        }

        if (AccessibilityFilterManager.Instance.IsGrayscaleOn())
        {
            ApplyGrayscale();
        }
        else if (AccessibilityFilterManager.Instance.IsHighContrastOn())
        {
            ApplyHighContrast();
        }
        else
        {
            ApplyNormal();
        }
    }

    private void ApplyNormal()
    {
        if (!hasCachedOriginals) return;

        if (cardBackground != null)
        {
            cardBackground.color = originalBackgroundColor;
        }

        if (rankImage != null)
        {
            rankImage.color = originalRankImageColor;
        }

        if (suitImage != null)
        {
            suitImage.color = originalSuitImageColor;
        }

        if (cardText != null)
        {
            cardText.color = originalCardTextColor;
        }

        if (rankText != null)
        {
            rankText.color = originalRankTextColor;
        }

        if (suitText != null)
        {
            suitText.color = originalSuitTextColor;
        }

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void ApplyGrayscale()
    {
        if (cardBackground != null)
        {
            float gray = ToGray(originalBackgroundColor);
            cardBackground.color = new Color(gray, gray, gray, originalBackgroundColor.a);
        }

        if (rankImage != null)
        {
            float gray = ToGray(originalRankImageColor);
            rankImage.color = new Color(gray, gray, gray, originalRankImageColor.a);
        }

        if (suitImage != null)
        {
            float gray = ToGray(originalSuitImageColor);
            suitImage.color = new Color(gray, gray, gray, originalSuitImageColor.a);
        }

        if (cardText != null)
        {
            cardText.color = Color.black;
        }

        if (rankText != null)
        {
            rankText.color = Color.black;
        }

        if (suitText != null)
        {
            suitText.color = Color.black;
        }

        if (outline != null)
        {
            outline.enabled = false;
        }
    }
    private void OnEnable()
    {
        Invoke(nameof(ApplyAccessibility), 0.01f);
    }

    private void ApplyHighContrast()
    {
        if (cardBackground != null)
        {
            float gray = ToGray(originalBackgroundColor);
            float boosted = gray > 0.5f ? 1f : 0.15f;
            cardBackground.color = new Color(boosted, boosted, boosted, originalBackgroundColor.a);
        }

        if (rankImage != null)
        {
            rankImage.color = Color.black;
        }

        if (suitImage != null)
        {
            suitImage.color = Color.black;
        }

        if (cardText != null)
        {
            cardText.color = Color.black;
        }

        if (rankText != null)
        {
            rankText.color = Color.black;
        }

        if (suitText != null)
        {
            suitText.color = Color.black;
        }

        if (outline != null)
        {
            outline.enabled = true;
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(3f, 3f);
        }
    }
}