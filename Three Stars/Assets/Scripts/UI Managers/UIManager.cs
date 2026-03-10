using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Coroutine scoreScaleCoroutine;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreThresholdText;

    [Header("Shift Info UI")]
    public TextMeshProUGUI playsText;
    public TextMeshProUGUI discardsText;
    public TextMeshProUGUI shiftNumberText;

    [Header("Score X Mult")]
    public TextMeshProUGUI handScoreText;
    public TextMeshProUGUI multText;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        ShiftManager.OnUIUpdate += UpdateAllUI;
    }

    private void OnDisable()
    {
        ShiftManager.OnUIUpdate -= UpdateAllUI;
    }

    private void UpdateAllUI()
    {
        var shiftManager = GameManager.Instance.shiftManager;
        scoreText.text = $"Current Score: {shiftManager.score}";
        scoreThresholdText.text = $"Goal: {shiftManager.scoreThreshold}";
        playsText.text = $"Plays: {shiftManager.plays}";
        discardsText.text = $"Discards: {shiftManager.discards}";
        shiftNumberText.text = $"Shift: {shiftManager.shiftNumber + 1}";
    }

    public void UpdateScoreXMultScore(float pips)
    {
        if (handScoreText != null)
        {
            handScoreText.text = pips.ToString();

            if (scoreScaleCoroutine != null)
            {
                StopCoroutine(scoreScaleCoroutine);
            }
            scoreScaleCoroutine = StartCoroutine(LerpScale(handScoreText.rectTransform, 1.1f, 0.1f));
        }
    }

    public void UpdateScoreXMultMult(float mult)
    {
        if (multText != null)
        {
            multText.text = mult.ToString();

            if (scoreScaleCoroutine != null)
            {
                StopCoroutine(scoreScaleCoroutine);
            }
            scoreScaleCoroutine = StartCoroutine(LerpScale(multText.rectTransform, 1.1f, 0.1f));
        }
    }

    private IEnumerator LerpScale(RectTransform target, float targetScale, float duration)
    {
        Vector3 originalScale = Vector3.one;
        Vector3 upScale = new Vector3(targetScale, targetScale, 1f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.localScale = Vector3.Lerp(originalScale, upScale, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = upScale;

        elapsed = 0f;
        while(elapsed < duration)
        {
            target.localScale = Vector3.Lerp(upScale, originalScale, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = originalScale;
        scoreScaleCoroutine = null;
    }
}
