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

    // Fix layout: ensure the row itself stretches horizontally
    if (transform is RectTransform rt)
    {
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(1, 0.5f);
        rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);
    }

    // Add a HorizontalLayoutGroup if missing to spread out children
    HorizontalLayoutGroup hlg = GetComponent<HorizontalLayoutGroup>();
    if (hlg == null)
    {
        hlg = gameObject.AddComponent<HorizontalLayoutGroup>();
        hlg.childControlWidth = true;
        hlg.childForceExpandWidth = true;
        hlg.padding = new RectOffset(10, 10, 5, 5);
        hlg.spacing = 10;
    }

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