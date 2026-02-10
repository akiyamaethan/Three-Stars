using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [Header("Main Stats")]
    [SerializeField] private TMP_Text targetScoreValue;
    [SerializeField] private TMP_Text cashValue;
    [SerializeField] private TMP_Text yourScoreValue;

    [Header("Score x Mult")]
    [SerializeField] private TMP_Text scoreValue;   // left number
    [SerializeField] private TMP_Text multValue;    // right number (just the multiplier number)

    [Header("Mini Stats")]
    [SerializeField] private TMP_Text discardsValue;
    [SerializeField] private TMP_Text handsValue;

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

        // Display like: "123 x 4"
        if (scoreValue != null) scoreValue.text = scoreBase.ToString();
        if (multValue != null) multValue.text = mult.ToString();

        if (discardsValue != null) discardsValue.text = discardsRemaining.ToString();
        if (handsValue != null) handsValue.text = handsRemaining.ToString();
    }
}
