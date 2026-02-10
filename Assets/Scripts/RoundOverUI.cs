using TMPro;
using UnityEngine;

public class RoundOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;

    public void Show(bool win, int score, int target)
    {
        if (panel != null) panel.SetActive(true);
        if (resultText != null)
        {
            resultText.text = win
                ? $"YOU WIN!\nScore: {score} / {target}"
                : $"YOU LOSE\nScore: {score} / {target}";
        }
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }
}
