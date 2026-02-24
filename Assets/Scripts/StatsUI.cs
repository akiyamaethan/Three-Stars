using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [Header("Main Stats")]
    [SerializeField] private TMP_Text targetScoreValue;
    [SerializeField] private TMP_Text cashValue;
    [SerializeField] private TMP_Text yourScoreValue;

    [Header("Score x Mult")]
    [SerializeField] private TMP_Text scoreValue;
    [SerializeField] private TMP_Text multValue;

    [Header("Mini Stats")]
    [SerializeField] private TMP_Text discardsValue;
    [SerializeField] private TMP_Text handsValue;

    [Header("Blind Info (NEW)")]
    [SerializeField] private TMP_Text blindValue;
    [SerializeField] private TMP_Text difficultyValue;

    public void Refresh(
        int targetScore,
        int cash,
        int score,
        int scoreBase,
        int mult,
        int discardsRemaining,
        int handsRemaining)
    {
        if (targetScoreValue != null) targetScoreValue.text = targetScore.ToString();
        if (cashValue != null) cashValue.text = $"${cash}";
        if (yourScoreValue != null) yourScoreValue.text = score.ToString();

        if (scoreValue != null) scoreValue.text = scoreBase.ToString();
        if (multValue != null) multValue.text = mult.ToString();

        if (discardsValue != null) discardsValue.text = discardsRemaining.ToString();
        if (handsValue != null) handsValue.text = handsRemaining.ToString();

        if (RunManager.Instance != null)
        {
            if (blindValue != null)
                blindValue.text = RunManager.Instance.RoundNumber.ToString();

            if (difficultyValue != null)
                difficultyValue.text = RunManager.Instance.SelectedDifficulty.ToString();
        }
    }
}