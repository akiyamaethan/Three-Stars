using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    //Singleton setup
    public static GameManager Instance { get; private set; }
    //Other managers
    public DeckManager deckManager { get; private set; }
    public ThreeStars.ProgressionManager progressionManager { get; private set; }
    public ScoreManager scoreManager { get; private set; }
    public ShiftManager shiftManager { get; private set; }
    public ShopManager shopManager { get; private set; }

    //Game states
    public enum GameState
    {
        MainMenu,
        Playing,
        InShop,
        GameOver
    }

    // UI References
    public RectTransform DiscardPileTransform;
    public GameState currentState;
    public Canvas gameplayCanvas;
    public Canvas shopCanvas;

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
            SwitchToState(GameState.Playing);
        }
    }
    private void Start()
    {
        if (Instance == this && shiftManager != null)
        {
            shiftManager.ResetShift(); //Starts the game
        }
    }

    public void SwitchToState(GameState state)
    {
        currentState = state;
        if (gameplayCanvas != null) gameplayCanvas.gameObject.SetActive(state == GameState.Playing);
        if (shopCanvas != null) shopCanvas.gameObject.SetActive(state == GameState.InShop);
    }


    private void InitializeManagers()
    {
        // Use GetComponentInChildren first, then fallback to Resources/Instantiate
        deckManager = GetComponentInChildren<DeckManager>();
        if (deckManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Deck Manager");
            if (prefab != null)
            {
                GameObject deckManagerObj = Instantiate(prefab, transform);
                deckManager = deckManagerObj.GetComponent<DeckManager>();
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
        }

        shopManager = GetComponentInChildren<ShopManager>();
        if (shopManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Shop Manager");
            if (prefab != null)
            {
                GameObject shopManagerObj = Instantiate(prefab, transform);
                shopManager = shopManagerObj.GetComponent<ShopManager>();
            }
        }

        // Final pass: ensure sub-managers have what they need if they were found in children
        if (shiftManager != null)
        {
            if (shiftManager.deckManager == null) shiftManager.deckManager = deckManager;
            
            HandManager handManager = FindFirstObjectByType<HandManager>(FindObjectsInactive.Include);
            if (handManager != null)
            {
                shiftManager.handManager = handManager;
                if (handManager.deckManager == null) handManager.deckManager = deckManager;
            }
            else
            {
                Debug.LogWarning("GameManager: Could not find HandManager in scene (even inactive)!");
            }
        }

        // Initialize CardModalController if present
        CardModalController cardModal = GetComponent<CardModalController>();
        if (cardModal != null)
        {
            // Find the root in scene if not assigned
            var root = GameObject.Find("CardModalRoot");
            if (root != null)
            {
                // Use reflection or a public setter if we don't want to change private fields, 
                // but since I can change the script, I'll just make it find it in its own Start or here.
                // For now, I'll update CardModalController.cs to be more self-sufficient.
            }
        }
    }
}

