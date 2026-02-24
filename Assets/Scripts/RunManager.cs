using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public Difficulty SelectedDifficulty { get; private set; } = Difficulty.Normal;

    public int RoundNumber { get; private set; } = 1;

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

    public void SetDifficulty(Difficulty difficulty)
    {
        SelectedDifficulty = difficulty;
        Debug.Log("Difficulty set to: " + SelectedDifficulty);
    }

    public void StartNewRun()
    {
        RoundNumber = 1;
        Debug.Log("New run started.");
    }

    public void NextRound()
    {
        RoundNumber++;
        Debug.Log("Advanced to round: " + RoundNumber);
    }

    public void GetCurrentRoundSettings(out int hands, out int discards, out int targetScore)
    {
        int baseHands;
        int baseDiscards;
        int baseTarget;
        float growth; // target multiplier per round

        switch (SelectedDifficulty)
        {
            case Difficulty.Easy:
                baseHands = 6;
                baseDiscards = 4;
                baseTarget = 80;
                growth = 1.12f;
                break;

            case Difficulty.Hard:
                baseHands = 4;
                baseDiscards = 2;
                baseTarget = 120;
                growth = 1.18f;
                break;

            default: // Normal
                baseHands = 5;
                baseDiscards = 3;
                baseTarget = 100;
                growth = 1.15f;
                break;
        }

        // Target grows each blind
        targetScore = Mathf.RoundToInt(baseTarget * Mathf.Pow(growth, RoundNumber - 1));

        // Hands/discards can slowly get harder (optional, simple rule)
        hands = baseHands;
        discards = baseDiscards;

        // Every 5 rounds, reduce one discard (down to a minimum of 1)
        if (RoundNumber >= 6)
            discards = Mathf.Max(1, baseDiscards - (RoundNumber - 1) / 5);
    }
}