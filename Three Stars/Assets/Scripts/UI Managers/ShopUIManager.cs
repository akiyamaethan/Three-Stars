using TMPro;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    public TextMeshProUGUI previousShiftText;
    public TextMeshProUGUI walletText;

    private void OnEnable()
    {
        ShopManager.OnShopUIUpdate += UpdateAllShopUI;
    }

    private void OnDisable()
    {
        ShopManager.OnShopUIUpdate -= UpdateAllShopUI;
    }

    private void UpdateAllShopUI()
    {
        var progressionManager = GameManager.Instance.progressionManager;
        previousShiftText.text = $"Shift {progressionManager.shiftNumber} Cleared!";
        walletText.text = $"Wallet: ${progressionManager.playerBalance}";
    }
}
