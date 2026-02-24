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
    HandleWalletChanged(ShopManager.Instance != null ? ShopManager.Instance.Wallet : 0);
}
private void HandleWalletChanged(int amount)
{
    walletText.text = $"Wallet: {amount}";
}
    private void UpdateAllUI()
    {
        var shiftManager = ShiftManager.Instance;
        scoreText.text = $"Current Score: {shiftManager.score}";
        scoreThresholdText.text = $"Goal: {shiftManager.scoreThreshold}";
        playsText.text = $"Plays: {shiftManager.plays}";
        discardsText.text = $"Discards: {shiftManager.discards}";
        shiftNumberText.text = $"Shift: {shiftManager.shiftNumber + 1}";
    }
}
