using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreThresholdText;

    [Header("Shift Info UI")]
    public TextMeshProUGUI playsText;
    public TextMeshProUGUI discardsText;
    public TextMeshProUGUI shiftNumberText;

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
}
