using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class ShiftManager : MonoBehaviour
{
    public bool debugMode = true;

    // Shift variables
    public int shiftNumber = 0;
    public int discards = 0;
    public int plays = 0;
    public int scoreThreshold = 0;
    public int score = 0;
    public int prevScore = 0;

    // Events
    public static event System.Action OnGameOver;
    public static event System.Action OnUIUpdate;

    // Managers
    public DeckManager deckManager;
    public HandManager handManager;

    [SerializeField] private ResultsPanelController resultsPanel;
    [SerializeField] private LossPanelController lossPanel;

    private readonly float[] possibleNextRoundScoreMults = new float[]
    {
        1.1f, 1.1f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.3f, 1.4f, 1.5f
    };

    // NEW: store every hand played this shift (and its score)
    private readonly List<List<CardInstance>> handsPlayedThisShift = new();
    private readonly List<int> handScoresThisShift = new();

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

        // Find even if disabled at start
        if (resultsPanel == null)
            resultsPanel = FindAnyObjectByType<ResultsPanelController>(FindObjectsInactive.Include);

        if (lossPanel == null)
            lossPanel = FindAnyObjectByType<LossPanelController>(FindObjectsInactive.Include);
    }

    private void RefreshUI()
    {
        OnUIUpdate?.Invoke();
    }

    private void OnHandPlayed(List<CardInstance> hand, int handScore)
    {
        // NEW: record this hand (copy list so it doesn't get mutated later)
        var copy = (hand != null) ? new List<CardInstance>(hand) : new List<CardInstance>();
        handsPlayedThisShift.Add(copy);
        handScoresThisShift.Add(handScore);

        plays--;
        score += handScore;

        if (debugMode)
            Debug.Log($"Current score: {score} / {scoreThreshold}");

        if (score >= scoreThreshold)
        {
            if (debugMode) Debug.Log($"Shift {shiftNumber} complete! Score: {score}");

            // Advance shift in progression
            GameManager.Instance.progressionManager.shiftNumber++;

            // Update score history for threshold calculation
            UpdatePreviousScores();

            // Add overflow to wallet
            if (ShopManager.Instance != null)
                ShopManager.Instance.AddOverflowFromShift(score, scoreThreshold, cleared: true);

            // Show win results (do NOT open shop yet)
            ShowWinResultsOrFallbackToShop();
        }
        else if (plays == 0)
        {
            if (debugMode) Debug.Log($"Shift failed. Final score: {score} / {scoreThreshold}");

            OnGameOver?.Invoke();
            ShowLossOrFallbackToGameOver();
        }

        RefreshUI();
    }

    private void ShowWinResultsOrFallbackToShop()
    {
        // Keep gameplay canvas active so results can show
        GameManager.Instance.SwitchToState(GameManager.GameState.Playing);

        if (resultsPanel == null)
        {
            Debug.LogWarning("[ShiftManager] ResultsPanelController not found. Falling back to opening Shop.");
            OpenShop();
            return;
        }

        // NEW: build a list of lines, one line per hand played
        List<string> handLines = BuildHandsSummaryLines(handsPlayedThisShift, handScoresThisShift);

        // Show summary lines on the win screen
        resultsPanel.Show(score, scoreThreshold, handLines);
    }

    private void ShowLossOrFallbackToGameOver()
    {
        GameManager.Instance.SwitchToState(GameManager.GameState.Playing);

        if (lossPanel == null)
        {
            Debug.LogWarning("[ShiftManager] LossPanelController not found. Falling back to GameOver state.");
            GameManager.Instance.SwitchToState(GameManager.GameState.GameOver);
            return;
        }

        lossPanel.Show(score, scoreThreshold);
    }

    private List<string> BuildHandsSummaryLines(List<List<CardInstance>> hands, List<int> scores)
    {
        List<string> lines = new();

        for (int i = 0; i < hands.Count; i++)
        {
            var hand = hands[i];
            int handScore = (i < scores.Count) ? scores[i] : 0;

            List<string> cardParts = new();

            if (hand != null)
            {
                foreach (var ci in hand)
                {
                    if (ci?.cardData == null)
                    {
                        cardParts.Add("Unknown");
                        continue;
                    }

                    // Using your PlayingCard fields
                    cardParts.Add($"{ci.cardData.cardRank} of {ci.cardData.cardSuit}");
                }
            }

            string cardsJoined = string.Join(", ", cardParts);
            lines.Add($"Hand {i + 1} (+{handScore}): {cardsJoined}");
        }

        return lines;
    }

    private void OpenShop()
    {
        GameManager.Instance.SwitchToState(GameManager.GameState.InShop);
        if (ShopManager.Instance != null)
            ShopManager.Instance.NotifyShopAvailable();
    }

    public void StartNextShift()
    {
        GameManager.Instance.SwitchToState(GameManager.GameState.Playing);
        ResetShift();
    }

    public void ResetShift()
    {
        // NEW: clear history at the start of every shift
        handsPlayedThisShift.Clear();
        handScoresThisShift.Clear();

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

        if (debugMode)
            Debug.Log($"Updated previous scores: prevScore: {GameManager.Instance.progressionManager.prevScore}, prevPrevScore: {GameManager.Instance.progressionManager.prevPrevScore}, prevPrevPrevScore: {GameManager.Instance.progressionManager.prevPrevPrevScore}");
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

        float newScoreMult = possibleNextRoundScoreMults[Random.Range(0, possibleNextRoundScoreMults.Length)];
        if (debugMode) Debug.Log($"Previous score: {prevScore}, new score multiplier: {newScoreMult}");

        if (shiftNumber < 10)
        {
            float rawThreshold = prevScore * newScoreMult;
            scoreThreshold = Mathf.RoundToInt(rawThreshold / 10f) * 10;
        }
        else if (shiftNumber > 9 && shiftNumber < 20)
        {
            float rawThreshold = prevScore * newScoreMult;
            scoreThreshold = Mathf.RoundToInt(rawThreshold / 100f) * 100;
        }
        else if (shiftNumber > 19)
        {
            float rawThreshold = prevScore * newScoreMult;
            scoreThreshold = Mathf.RoundToInt(rawThreshold / 1000f) * 1000;
        }

        if (debugMode) Debug.Log($"Shift: {shiftNumber}. New score threshold: {scoreThreshold}");
    }

    public void TriggerDiscard()
    {
        discards--;
        if (debugMode) Debug.Log($"Discard triggered! Total discards: {discards} / {GameManager.Instance.progressionManager.discards}");
        RefreshUI();
    }
}