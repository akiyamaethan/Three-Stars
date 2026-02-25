using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LossPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartRun);
    }

    public void Show(int score, int target)
    {
        gameObject.SetActive(true);

        if (titleText != null) titleText.text = "You Lost";
        if (bodyText != null) bodyText.text = "You did not reach the quota.";
        if (scoreText != null) scoreText.text = $"Score: {score} / Target: {target}";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RestartRun()
    {
        // Minimum viable restart:
        // reset progression + shop, then start shift 1 again
        if (ShopManager.Instance != null)
            ShopManager.Instance.ResetForNewRun();

        if (GameManager.Instance != null && GameManager.Instance.progressionManager != null)
            GameManager.Instance.progressionManager.ResetForNewRun(); // only if you have this method

        // If you DON'T have ResetForNewRun on progressionManager, comment that out and do a scene reload instead.

        if (GameManager.Instance != null && GameManager.Instance.shiftManager != null)
        {
            GameManager.Instance.shiftManager.StartNextShift();
        }

        Hide();
    }
}