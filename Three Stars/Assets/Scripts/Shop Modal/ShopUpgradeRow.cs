using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgradeRow : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI handNameText;
    public TextMeshProUGUI multText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    private HandEvaluator.HandRank rank;
    private ShopModalController modal;

    public void Bind(ShopModalController modalController, HandEvaluator.HandRank handRank)
{
    modal = modalController;
    rank = handRank;

    if (buyButton == null)
    {
        Debug.LogError($"[ShopUpgradeRow] buyButton is not assigned on prefab '{gameObject.name}'.");
        return;
    }

    buyButton.onClick.RemoveAllListeners();
    buyButton.onClick.AddListener(() => modal.TryBuy(rank));
}

    public void Refresh(string handName, float mult, int cost, bool canAfford)
    {
        handNameText.text = handName;
        multText.text = $"x{mult:0.00}";
        costText.text = $"Cost: {cost}";
        buyButton.interactable = canAfford;
    }
}