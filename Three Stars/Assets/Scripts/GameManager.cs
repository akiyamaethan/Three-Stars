using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DeckManager deckManager { get; private set; }
    public ThreeStars.ProgressionManager progressionManager { get; private set; }
    public ScoreManager scoreManager { get; private set; }
    public ShiftManager shiftManager { get; private set; }
    public static GameManager Instance { get; private set; }

    public RectTransform DiscardPileTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
    }

    private void Start()
    {
        shiftManager.ResetShift(); //Starts the game
    }

    private void InitializeManagers()
    {
        deckManager = GetComponentInChildren<DeckManager>();

        if (deckManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Deck Manager");
            if (prefab != null)
            {
                GameObject deckManagerObj = Instantiate(prefab, transform);
                deckManager = deckManagerObj.GetComponent<DeckManager>();
            }
            else
            {
                Debug.LogError("Deck Manager prefab not found in Resources/Prefabs.");
            }
        }

        progressionManager = GetComponentInChildren<ThreeStars.ProgressionManager>();
        if (progressionManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Progression Manager");
            if (prefab != null)
            {
                GameObject progressionManagerObj = Instantiate(prefab, transform);
                progressionManager = progressionManagerObj.GetComponent<ThreeStars.ProgressionManager>();
            }
            else
            {
                Debug.LogError("Progression Manager prefab not found in Resources/Prefabs.");
            }
        }

        scoreManager = GetComponentInChildren<ScoreManager>();
        if (scoreManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Score Manager");
            if (prefab != null)
            {
                GameObject scoreManagerObj = Instantiate(prefab, transform);
                scoreManager = scoreManagerObj.GetComponent<ScoreManager>();
            }
            else
            {
                Debug.LogError("Score Manager prefab not found in Resources/Prefabs.");
            }
        }

        shiftManager = GetComponentInChildren<ShiftManager>();
        if (shiftManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Shift Manager");
            if (prefab != null)
            {
                GameObject shiftManagerObj = Instantiate(prefab, transform);
                shiftManager = shiftManagerObj.GetComponent<ShiftManager>();
            }
            else
            {
                Debug.LogError("Shift Manager prefab not found in Resources/Prefabs.");
            }
        }
    }
}

