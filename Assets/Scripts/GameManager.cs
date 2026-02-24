// Assets/Scripts/GameManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Hand Settings")]
    [SerializeField] private int handSize = 7;

    [Header("Round Settings (defaults if RunManager missing)")]
    [SerializeField] private int startingHands = 5;
    [SerializeField] private int startingDiscards = 3;
    [SerializeField] private int targetScore = 100;
    [SerializeField] private int startingCash = 20;

    [Header("UI")]
    [SerializeField] private HandUI handUI;
    [SerializeField] private StatsUI statsUI;

    [Header("Selection")]
    [SerializeField] private SelectionManager selectionManager;

    [Header("Round Over")]
    [SerializeField] private RoundOverUI roundOverUI;

    private Deck deck;

    // Fixed slots: null means empty slot
    private CardData[] handSlots;

    // Permanently removed cards (never go back to the deck)
    private readonly List<CardData> playedPile = new();

    // Cache the last played cards for scoring (comes from SelectionManager.OnPlayedCards)
    private List<CardData> mostRecentPlayedCards = new();

    private int handsRemaining;
    private int discardsRemaining;
    private int currentScore;

    // For UI "Score x Mult"
    private int scoreBaseThisHand;
    private int multThisHand;

    // For UI cash
    private int cash;

    public int HandsRemaining => handsRemaining;
    public int DiscardsRemaining => discardsRemaining;

    private void Start()
    {
        // --- NEW: pull settings from RunManager (MenuScene) if it exists ---
        if (RunManager.Instance != null)
        {
            RunManager.Instance.GetCurrentRoundSettings(out startingHands, out startingDiscards, out targetScore);

            Debug.Log(
                $"Loaded round settings from RunManager: " +
                $"difficulty={RunManager.Instance.SelectedDifficulty}, " +
                $"round={RunManager.Instance.RoundNumber}, " +
                $"hands={startingHands}, discards={startingDiscards}, target={targetScore}");
        }
        else
        {
            Debug.Log("RunManager not found. Using default round settings from GameManager Inspector.");
        }
        // ---------------------------------------------------------------

        // Init round stats
        handsRemaining = startingHands;
        discardsRemaining = startingDiscards;
        currentScore = 0;

        cash = startingCash;

        // For now, mult is always 1
        scoreBaseThisHand = 0;
        multThisHand = 1;

        // Build deck and initial hand
        handSlots = new CardData[handSize];

        deck = new Deck();
        deck.Build52();
        deck.Shuffle();

        FillAllEmptySlots();

        if (handUI != null)
        {
            handUI.RenderHandSlots(handSlots);
        }
        else
        {
            Debug.LogError("GameManager: HandUI is not assigned in Inspector.");
        }

        // Subscribe to selection events
        if (selectionManager != null)
        {
            selectionManager.OnPlayedCards += CachePlayedCardsForScoring;
            selectionManager.OnPlayedSlotIndices += HandlePlayedSlots;
            selectionManager.OnDiscardSlotIndices += HandleDiscardSlots;
        }
        else
        {
            Debug.LogError("GameManager: SelectionManager is not assigned in Inspector.");
        }

        RefreshUI();

        Debug.Log($"Round start. Target: {targetScore}. Hands: {handsRemaining}. Discards: {discardsRemaining}. Deck remaining: {deck.Count}");
    }

    private void CachePlayedCardsForScoring(List<CardData> cards)
    {
        mostRecentPlayedCards = cards ?? new List<CardData>();
    }

    private void HandlePlayedSlots(List<int> slotIndices)
    {
        if (slotIndices == null || slotIndices.Count == 0) return;

        // If round already ended, ignore plays
        if (handsRemaining <= 0) return;

        // Consume a hand
        handsRemaining--;

        // Score based on what was played
        int gained = ScoreRules.ScorePlayedCards(mostRecentPlayedCards);
        currentScore += gained;

        // Update "Score x Mult" for the UI
        scoreBaseThisHand = gained;
        multThisHand = 1;

        Debug.Log($"Play: +{gained} points. Total: {currentScore}. Hands left: {handsRemaining}/{startingHands}.");

        // Remove cards from those exact slots
        RemoveCardsFromSlots(slotIndices);

        // Refill only empty slots (no shifting)
        FillAllEmptySlots();

        // Re-render hand
        if (handUI != null)
        {
            handUI.RenderHandSlots(handSlots);
        }

        RefreshUI();

        // End-of-round check
        if (handsRemaining <= 0)
        {
            bool win = currentScore >= targetScore;
            Debug.Log(win ? "ROUND WIN" : "ROUND LOSE");

            if (roundOverUI != null)
                roundOverUI.Show(win, currentScore, targetScore);
        }
    }
    public void ContinueAfterWin()
    {
        if (RunManager.Instance != null)
            RunManager.Instance.NextRound();

        SceneManager.LoadScene("gamescene");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void RestartRound()
    {
        // Reset stats
        handsRemaining = startingHands;
        discardsRemaining = startingDiscards;
        currentScore = 0;

        scoreBaseThisHand = 0;
        multThisHand = 1;

        // Rebuild deck
        deck = new Deck();
        deck.Build52();
        deck.Shuffle();

        // Clear hand slots
        for (int i = 0; i < handSlots.Length; i++)
            handSlots[i] = null;

        // Deal
        FillAllEmptySlots();

        // Render
        if (handUI != null)
            handUI.RenderHandSlots(handSlots);

        RefreshUI();

        // Hide overlay
        if (roundOverUI != null)
            roundOverUI.Hide();

        Debug.Log("Round restarted.");
    }

    private void HandleDiscardSlots(List<int> slotIndices)
    {
        if (slotIndices == null || slotIndices.Count == 0) return;

        // No discards left
        if (discardsRemaining <= 0) return;

        discardsRemaining--;

        // Discard does not score, but we can clear the "Score x Mult" display
        scoreBaseThisHand = 0;
        multThisHand = 1;

        Debug.Log($"Discarded {slotIndices.Count}. Discards left: {discardsRemaining}/{startingDiscards}.");

        // Remove cards from those exact slots
        RemoveCardsFromSlots(slotIndices);

        // Refill only empty slots
        FillAllEmptySlots();

        // Re-render hand
        if (handUI != null)
        {
            handUI.RenderHandSlots(handSlots);
        }

        RefreshUI();
    }

    private void RemoveCardsFromSlots(List<int> slotIndices)
    {
        foreach (int idx in slotIndices)
        {
            if (idx < 0 || idx >= handSlots.Length) continue;

            var removed = handSlots[idx];
            if (removed != null)
            {
                playedPile.Add(removed); // permanently removed from game
                handSlots[idx] = null;
            }
        }
    }

    private void RefreshUI()
    {
        if (statsUI == null) return;

        statsUI.Refresh(
            targetScore: targetScore,
            cash: cash,
            score: currentScore,
            scoreBase: scoreBaseThisHand,
            mult: multThisHand,
            discardsRemaining: discardsRemaining,
            handsRemaining: handsRemaining
        );
    }

    private void FillAllEmptySlots()
    {
        for (int i = 0; i < handSlots.Length; i++)
        {
            if (handSlots[i] == null)
            {
                var c = deck.Draw();
                if (c == null)
                {
                    Debug.Log("Deck is empty. No more cards can be dealt.");
                    return;
                }

                handSlots[i] = c;
            }
        }
    }
}