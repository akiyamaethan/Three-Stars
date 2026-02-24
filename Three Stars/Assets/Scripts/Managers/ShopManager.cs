using System;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    // EVENTS (your UI depends on these)
    public static event Action OnShopAvailable;
    public static event Action<int> OnWalletChanged;
    public static event Action OnShopChanged;

    public int Wallet { get; private set; } = 0;

    // how many upgrades purchased per hand type (drives the exponential cost)
    private readonly Dictionary<HandEvaluator.HandRank, int> handUpgradeLevels = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddOverflowFromShift(int finalScore, int threshold, bool cleared)
    {
        if (!cleared) return;

        int overflow = finalScore - threshold;
        if (overflow <= 0) return;

        Wallet += overflow;
        OnWalletChanged?.Invoke(Wallet);
    }

    public bool CanAfford(int cost) => cost >= 0 && Wallet >= cost;

    public bool TrySpend(int cost)
    {
        if (cost < 0) return false;
        if (Wallet < cost) return false;

        Wallet -= cost;
        OnWalletChanged?.Invoke(Wallet);
        return true;
    }

    // Keep ONLY one copy of this method
    public void NotifyShopAvailable()
    {
        OnShopAvailable?.Invoke();
    }

    public void ResetForNewRun()
    {
        Wallet = 0;
        handUpgradeLevels.Clear();
        OnWalletChanged?.Invoke(Wallet);
        OnShopChanged?.Invoke();
    }

    // =========================
    // Shop Modal Support Methods
    // =========================

    public float GetCurrentMultiplier(HandEvaluator.HandRank rank)
    {
        var prog = ProgressionManager.Instance;
        if (prog == null) return 1f;

        return rank switch
        {
            HandEvaluator.HandRank.HighCard => prog.highCardMult,
            HandEvaluator.HandRank.Rainbow => prog.rainbowMult,
            HandEvaluator.HandRank.OnePair => prog.pairMult,
            HandEvaluator.HandRank.TwoPair => prog.twoPairMult,
            HandEvaluator.HandRank.ThreeOfAKind => prog.tripsMult,
            HandEvaluator.HandRank.Straight => prog.straightMult,
            HandEvaluator.HandRank.Flush => prog.flushMult,
            HandEvaluator.HandRank.FourOfAKind => prog.quadsMult,
            HandEvaluator.HandRank.StraightFlush => prog.straightFlushMult,
            HandEvaluator.HandRank.RoyalFlush => prog.royalFlushMult,
            _ => 1f
        };
    }

    public int GetCost(HandEvaluator.HandRank rank)
    {
        // Cookie clicker style: exponential growth
        int level = handUpgradeLevels.TryGetValue(rank, out int l) ? l : 0;

        const int baseCost = 20;
        const float growth = 1.5f; // tweak later (1.15–1.8 are common)

        float cost = baseCost * Mathf.Pow(growth, level);
        return Mathf.CeilToInt(cost);
    }

    public bool TryBuyHandUpgrade(HandEvaluator.HandRank rank)
    {
        int cost = GetCost(rank);
        if (!TrySpend(cost)) return false;

        var prog = ProgressionManager.Instance;
        if (prog == null) return false;

        // multiplier bump per purchase (tweak later)
        const float bump = 0.10f;

        switch (rank)
        {
            case HandEvaluator.HandRank.HighCard: prog.highCardMult += bump; break;
            case HandEvaluator.HandRank.Rainbow: prog.rainbowMult += bump; break;
            case HandEvaluator.HandRank.OnePair: prog.pairMult += bump; break;
            case HandEvaluator.HandRank.TwoPair: prog.twoPairMult += bump; break;
            case HandEvaluator.HandRank.ThreeOfAKind: prog.tripsMult += bump; break;
            case HandEvaluator.HandRank.Straight: prog.straightMult += bump; break;
            case HandEvaluator.HandRank.Flush: prog.flushMult += bump; break;
            case HandEvaluator.HandRank.FourOfAKind: prog.quadsMult += bump; break;
            case HandEvaluator.HandRank.StraightFlush: prog.straightFlushMult += bump; break;
            case HandEvaluator.HandRank.RoyalFlush: prog.royalFlushMult += bump; break;
            default:
                return false;
        }

        // record purchase level for pricing
        handUpgradeLevels[rank] = handUpgradeLevels.TryGetValue(rank, out int l) ? l + 1 : 1;

        // IMPORTANT: tell UI to refresh costs + multipliers
        OnShopChanged?.Invoke();
        return true;
    }
}