using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopModalController : MonoBehaviour
{
    [Header("Root")]
    public GameObject shopModalRoot;

    [Header("Top UI")]
    public TextMeshProUGUI walletText;

    [Header("List")]
    public Transform contentParent;
    public ShopUpgradeRow rowPrefab;

    // Keep row refs so we can refresh after buying
    private readonly Dictionary<HandEvaluator.HandRank, ShopUpgradeRow> rows = new();

    private static readonly HandEvaluator.HandRank[] ShopRanks =
    {
        HandEvaluator.HandRank.HighCard,
        HandEvaluator.HandRank.Rainbow,
        HandEvaluator.HandRank.OnePair,
        HandEvaluator.HandRank.TwoPair,
        HandEvaluator.HandRank.ThreeOfAKind,
        HandEvaluator.HandRank.Straight,
        HandEvaluator.HandRank.Flush,
        HandEvaluator.HandRank.FourOfAKind,
        HandEvaluator.HandRank.StraightFlush,
        HandEvaluator.HandRank.RoyalFlush,
    };

    private void OnEnable()
    {
        ShopManager.OnShopAvailable += Open;
        ShopManager.OnWalletChanged += OnWalletChanged;
    }

    private void OnDisable()
    {
        ShopManager.OnShopAvailable -= Open;
        ShopManager.OnWalletChanged -= OnWalletChanged;
    }

    private void Start()
    {
        // Start hidden
        if (shopModalRoot != null) shopModalRoot.SetActive(false);
    }

    private void OnWalletChanged(int newWallet)
    {
        RefreshAll();
    }

    public void Open()
    {
        if (shopModalRoot == null) return;
        shopModalRoot.SetActive(true);

        // Fix squashed layout: force root to be full screen
        if (shopModalRoot.transform is RectTransform rootRT)
        {
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.offsetMin = Vector2.zero;
            rootRT.offsetMax = Vector2.zero;
        }

        BuildIfNeeded();
        RefreshAll();

        // Force rebuild of the entire hierarchy to fix "skinny" columns
        Canvas.ForceUpdateCanvases();
        if (contentParent is RectTransform rt)
        {
            // Ensure the content parent itself is stretching to its parent's width
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = new Vector2(0, rt.offsetMin.y);
            rt.offsetMax = new Vector2(0, rt.offsetMax.y);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }

    public void Close()
    {
        if (shopModalRoot == null) return;
        shopModalRoot.SetActive(false);
    }

    public void CloseAndContinue()
    {
        Close();
        if (GameManager.Instance != null && GameManager.Instance.shiftManager != null)
        {
            GameManager.Instance.shiftManager.StartNextShift();
        }
    }

    private void BuildIfNeeded()
    {
        if (rows.Count > 0) return;

        foreach (var rank in ShopRanks)
        {
            var row = Instantiate(rowPrefab, contentParent);
            row.Bind(this, rank);
            rows.Add(rank, row);
        }
    }

    private void RefreshAll()
    {
        if (ShopManager.Instance == null) return;
        if (walletText != null) walletText.text = $"Wallet: {ShopManager.Instance.Wallet}";

        foreach (var kv in rows)
        {
            var rank = kv.Key;
            var row = kv.Value;

            float mult = ShopManager.Instance.GetCurrentMultiplier(rank);
            int cost = ShopManager.Instance.GetCost(rank);
            bool canAfford = ShopManager.Instance.CanAfford(cost);

            row.Refresh(rank.ToString(), mult, cost, canAfford);
        }
    }

    public void TryBuy(HandEvaluator.HandRank rank)
    {
        if (ShopManager.Instance == null) return;

        bool bought = ShopManager.Instance.TryBuyHandUpgrade(rank);
        if (bought)
        {
            RefreshAll();
        }
    }
}