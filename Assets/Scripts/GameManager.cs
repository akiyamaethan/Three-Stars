using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HandManager handManager { get; private set; }
    public DeckManager deckManager { get; private set; }
    public ThreeStars.ProgressionManager progressionManager { get; private set; }
    public ScoreManager scoreManager { get; private set; }
    public ShiftManager shiftManager { get; private set; }
    public static GameManager Instance { get; private set; }

    public ShopManager shopManager;

    public RectTransform DiscardPileTransform;

    public enum GameState
    {
        MainMenu,
        Playing,
        InShop,
        GameOver
    }

    public GameState currentState;
    public Canvas gameplayCanvas;
    public Canvas shopCanvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializeManagers();
    }

    private void Start()
    {
        SwitchToPlayState();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void InitializeManagers()
    {
        deckManager = GetOrLoadManager<DeckManager>("Prefabs/Deck Manager");
        progressionManager = GetOrLoadManager<ThreeStars.ProgressionManager>("Prefabs/Progression Manager");
        scoreManager = GetOrLoadManager<ScoreManager>("Prefabs/Score Manager");
        shiftManager = GetOrLoadManager<ShiftManager>("Prefabs/Shift Manager");
        shopManager = GetOrLoadManager<ShopManager>("Prefabs/Shop Manager");

        handManager = FindAnyObjectByType<HandManager>();
        HandEvaluator handEvaluator = FindAnyObjectByType<HandEvaluator>();

        if (handManager != null && handEvaluator != null)
        {
            handManager.Initialize(deckManager, handEvaluator);
        }
        else
        {
            Debug.LogError("HandManager or HandEvaluator not found in GameScene.");
        }

        if (shiftManager != null && handManager != null && deckManager != null && progressionManager != null)
        {
            shiftManager.Initialize(handManager, deckManager, progressionManager);
        }
        else
        {
            Debug.LogError("One or more managers required by ShiftManager are missing.");
        }

        if (deckManager != null)
        {
            deckManager.InitializeDeck();
        }
    }

    private T GetOrLoadManager<T>(string path) where T : MonoBehaviour
    {
        T manager = GetComponentInChildren<T>();

        if (manager == null)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            if (prefab != null)
            {
                GameObject managerObj = Instantiate(prefab, transform);
                manager = managerObj.GetComponent<T>();
            }
            else
            {
                Debug.LogError($"{typeof(T).Name} prefab not found in Resources/Prefabs.");
            }
        }

        return manager;
    }

    public void SwitchToPlayState()
    {
        currentState = GameState.Playing;

        if (gameplayCanvas != null)
            gameplayCanvas.gameObject.SetActive(true);

        if (shopCanvas != null)
            shopCanvas.gameObject.SetActive(false);

        if (shiftManager != null)
            shiftManager.ResetShift();
    }

    public void SwitchToShopState()
    {
        currentState = GameState.InShop;

        if (gameplayCanvas != null)
            gameplayCanvas.gameObject.SetActive(false);

        if (shopCanvas != null)
            shopCanvas.gameObject.SetActive(true);

        if (shopManager != null)
            shopManager.OpenShop();
    }
}