using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Wallet UI")]
public TextMeshProUGUI walletText;

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
        ShopManager.OnWalletChanged += HandleWalletChanged;
    }

    private void OnDisable()
    {
        ShiftManager.OnUIUpdate -= UpdateAllUI;
        ShopManager.OnWalletChanged -= HandleWalletChanged;
    }
    private void Start()
    {
        if (ShopManager.Instance != null)
        {
            HandleWalletChanged(ShopManager.Instance.Wallet);
        }
        else
        {
            HandleWalletChanged(0);
        }
    }

    private void HandleWalletChanged(int amount)
    {
        if (walletText != null)
        {
            walletText.text = $"Wallet: {amount}";
        }
    }

    private void UpdateAllUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.shiftManager == null) return;

        var shiftManager = GameManager.Instance.shiftManager;
        if (scoreText != null) scoreText.text = $"Current Score: {shiftManager.score}";
        if (scoreThresholdText != null) scoreThresholdText.text = $"Goal: {shiftManager.scoreThreshold}";
        if (playsText != null) playsText.text = $"Plays: {shiftManager.plays}";
        if (discardsText != null) discardsText.text = $"Discards: {shiftManager.discards}";
        if (shiftNumberText != null) shiftNumberText.text = $"Shift: {shiftManager.shiftNumber + 1}";
    }
}
