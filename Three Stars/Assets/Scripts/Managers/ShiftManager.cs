using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class ShiftManager : MonoBehaviour
{
    public static ShiftManager Instance { get; private set; }

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
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
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
        if (debugMode) Debug.Log($"Current score: {score} / {scoreThreshold}");
        if (score >= scoreThreshold)
        {
            if (debugMode) Debug.Log($"Shift {shiftNumber} complete! Score: {score}");
            ProgressionManager.Instance.shiftNumber++;
            UpdatePreviousScores();
                    // 1) Add overflow to wallet
        ShopManager.Instance.AddOverflowFromShift(score, scoreThreshold, cleared: true);

        // 2) Open shop
        ShopManager.Instance.NotifyShopAvailable();
            ResetShift();
        }
        if (plays == 0 && score < scoreThreshold)
        {
            OnGameOver?.Invoke();
            if (debugMode) Debug.Log($"Game Over! Final score: {score} / {scoreThreshold}");
        }
        if (debugMode) Debug.Log($"Hand played! Remaining plays: {plays} / {ProgressionManager.Instance.plays}");
        RefreshUI();
    }

    if (plays == 0 && score < scoreThreshold)
    {
        OnGameOver?.Invoke();
        if (debugMode)
            Debug.Log($"Game Over! Final score: {score} / {scoreThreshold}");
    }

    RefreshUI();
}
    public void ResetShift()
    {
        deckManager.Shuffle();
        handManager.ClearHand();
        handManager.DrawToFullHand();
        score = 0;
        shiftNumber = ProgressionManager.Instance.shiftNumber;
        discards = ProgressionManager.Instance.discards;
        plays = ProgressionManager.Instance.plays;
        CalculateScoreThreshold();
        RefreshUI();
    }

    public void UpdatePreviousScores()
    {
        ProgressionManager.Instance.prevPrevPrevScore = ProgressionManager.Instance.prevPrevScore;
        ProgressionManager.Instance.prevPrevScore = ProgressionManager.Instance.prevScore;
        ProgressionManager.Instance.prevScore = score;
        if (debugMode) Debug.Log($"Updated previous scores: prevScore: {ProgressionManager.Instance.prevScore}, prevPrevScore: {ProgressionManager.Instance.prevPrevScore}, prevPrevPrevScore: {ProgressionManager.Instance.prevPrevPrevScore}");
    }
    public void CalculateScoreThreshold()
    {
        prevScore = ProgressionManager.Instance.prevScore;
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
        if (debugMode) Debug.Log($"Discard triggered! Total discards: {discards} / {ProgressionManager.Instance.discards}");
        RefreshUI();
    }
}
