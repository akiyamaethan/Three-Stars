using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        BuildIfNeeded();
        RefreshAll();

        // Optional: pause input later (we’ll do next)
    }

    public void Close()
    {
        if (shopModalRoot == null) return;
        shopModalRoot.SetActive(false);
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