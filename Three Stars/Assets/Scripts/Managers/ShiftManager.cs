using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class ShiftManager : MonoBehaviour
{
    //debug
    public bool debugMode = true;

    //shift variables
    public int shiftNumber = 0;
    public int discards = 0;
    public int plays = 0;
    public int scoreThreshold = 0;
    public int score = 0;
    public int prevScore = 0; //used to determine next score threshold

    //events
    public static event System.Action OnGameOver;
    public static event System.Action OnUIUpdate;

    //managers
    public DeckManager deckManager;
    public HandManager handManager;

    //utilities
    private float[] possibleNextRoundScoreMults = new float[] { 1.1f, 1.1f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.3f, 1.4f, 1.5f };

    private void OnEnable()
    {
        HandManager.OnHandPlayed += OnHandPlayed;
    }

    private void OnDisable()
    {
        HandManager.OnHandPlayed -= OnHandPlayed;
    }
    private void Awake()
    {
        deckManager = FindAnyObjectByType<DeckManager>();
        handManager = FindAnyObjectByType<HandManager>();
    }

    private void RefreshUI()
    {
        OnUIUpdate?.Invoke();
    }
    private void OnHandPlayed(List<CardInstance> hand, int handScore)
{
    plays--;
    score += handScore;

    if (debugMode)
        Debug.Log($"Current score: {score} / {scoreThreshold}");

        if (score >= scoreThreshold)
        {
            if (debugMode) Debug.Log($"Shift {shiftNumber} complete! Score: {score}");
            GameManager.Instance.progressionManager.shiftNumber++;
            UpdatePreviousScores();
            // 1) Add overflow to wallet
            ShopManager.Instance.AddOverflowFromShift(score, scoreThreshold, cleared: true);

            // 2) Open shop and switch state
            GameManager.Instance.SwitchToState(GameManager.GameState.InShop);
            ShopManager.Instance.NotifyShopAvailable();
        }
        else if (plays == 0)
        {
            OnGameOver?.Invoke();
            GameManager.Instance.SwitchToState(GameManager.GameState.GameOver);
            if (debugMode) Debug.Log($"Game Over! Final score: {score} / {scoreThreshold}");
        }
        
        RefreshUI();
    }

    public void StartNextShift()
    {
        GameManager.Instance.SwitchToState(GameManager.GameState.Playing);
        ResetShift();
    }

    public void ResetShift()
    {
        if (deckManager != null) deckManager.Shuffle();
        if (handManager != null)
        {
            handManager.ClearHand();
            handManager.DrawToFullHand();
        }
        else
        {
            Debug.LogWarning("ShiftManager: handManager is null in ResetShift!");
        }
        
        score = 0;
        if (GameManager.Instance != null && GameManager.Instance.progressionManager != null)
        {
            shiftNumber = GameManager.Instance.progressionManager.shiftNumber;
            discards = GameManager.Instance.progressionManager.discards;
            plays = GameManager.Instance.progressionManager.plays;
        }
        
        CalculateScoreThreshold();
        RefreshUI();
    }

    public void UpdatePreviousScores()
    {
        GameManager.Instance.progressionManager.prevPrevPrevScore = GameManager.Instance.progressionManager.prevPrevScore;
        GameManager.Instance.progressionManager.prevPrevScore = GameManager.Instance.progressionManager.prevScore;
        GameManager.Instance.progressionManager.prevScore = score;
        if (debugMode) Debug.Log($"Updated previous scores: prevScore: {GameManager.Instance.progressionManager.prevScore}, prevPrevScore: {GameManager.Instance.progressionManager.prevPrevScore}, prevPrevPrevScore: {GameManager.Instance.progressionManager.prevPrevPrevScore}");
    }
    public void CalculateScoreThreshold()
    {
        prevScore = GameManager.Instance.progressionManager.prevScore;
        if (prevScore == 0)
        {
            if (debugMode) Debug.Log("First shift, setting score threshold to 100");
            scoreThreshold = 100;
            return;
        }
        else
        {
            float newScoreMult = possibleNextRoundScoreMults[Random.Range(0, possibleNextRoundScoreMults.Length)];
            if (debugMode) Debug.Log($"Previous score: {prevScore}, new score multiplier: {newScoreMult}");

            if (shiftNumber < 10)
            {
                float rawThreshold = prevScore * newScoreMult;
                scoreThreshold = Mathf.RoundToInt(rawThreshold / 10f) * 10; //rounds score to nearest 10
            }
            else if (shiftNumber > 9 && shiftNumber < 20)
            {
                float rawThreshold = prevScore * newScoreMult;
                scoreThreshold = Mathf.RoundToInt(rawThreshold / 100) * 100; //nearest 100
            }
            else if (shiftNumber > 19)
            {
                float rawThreshold = prevScore * newScoreMult;
                scoreThreshold = Mathf.RoundToInt(rawThreshold / 1000) * 1000; //nearest 1000
            }
            if (debugMode) Debug.Log($"Shift: {shiftNumber}. New score threshold: {scoreThreshold}");
        }
    }

    public void TriggerDiscard()
    {
        discards--;
        if (debugMode) Debug.Log($"Discard triggered! Total discards: {discards} / {GameManager.Instance.progressionManager.discards}");
        RefreshUI();
    }
}
