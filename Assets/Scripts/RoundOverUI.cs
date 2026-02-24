using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons (optional but recommended)")]
    [SerializeField] private Button primaryButton;      // Continue / Restart
    [SerializeField] private TMP_Text primaryButtonText;
    [SerializeField] private Button secondaryButton;    // Menu
    [SerializeField] private TMP_Text secondaryButtonText;

    [Header("Refs")]
    [SerializeField] private GameManager gameManager;

    private bool lastWin;

    public void Show(bool win, int score, int target)
    {
        lastWin = win;

        if (panel != null) panel.SetActive(true);

        if (resultText != null)
        {
            resultText.text = win
                ? $"YOU WIN!\nScore: {score} / {target}"
                : $"YOU LOSE\nScore: {score} / {target}";
        }

        // Configure buttons
        if (primaryButtonText != null)
            primaryButtonText.text = win ? "Next Blind" : "Retry";

        if (secondaryButtonText != null)
            secondaryButtonText.text = "Menu";

        // Optional: disable primary if missing GameManager
        if (primaryButton != null)
            primaryButton.interactable = (gameManager != null);

        if (secondaryButton != null)
            secondaryButton.interactable = true;
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    // Hook this to the Primary button OnClick
    public void OnPrimaryClicked()
    {
        if (gameManager == null) return;

        if (lastWin)
        {
            gameManager.ContinueAfterWin();  // loads next round
        }
        else
        {
            // You can choose what "Retry" means:
            // Option A: restart the current round with same settings
            gameManager.RestartRound();

            // Option B instead: restart the whole run
            // gameManager.ReturnToMenu();
        }
    }

    // Hook this to the Secondary button OnClick
    public void OnSecondaryClicked()
    {
        if (gameManager == null) return;
        gameManager.ReturnToMenu();
    }
}