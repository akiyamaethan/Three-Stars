using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine.SceneManagement;
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
        Instance = this;
        InitializeManagers();
    }

    private void Start()
    {
        SwitchToPlayState();
    }

    // Initializer
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
            shiftManager.Initialize(handManager, deckManager, progressionManager);
            deckManager.InitializeDeck();
        }
        else
        {
            Debug.LogWarning("Could not find a handmanager or evaluator in scene");
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
    Debug.Log("[GameManager] Switching to Play State.");

    currentState = GameState.Playing;
    gameplayCanvas.gameObject.SetActive(true);
    shopCanvas.gameObject.SetActive(false);

    if (SoundManager.Instance != null)
    {
        SoundManager.Instance.PlayNextShift();
        SoundManager.Instance.FadeBackToMusic();
    }
    else
    {
        Debug.LogWarning("[GameManager] SoundManager.Instance is null in SwitchToPlayState().");
    }

    shiftManager.ResetShift();
}

public void SwitchToShopState()
{
    currentState = GameState.InShop;
    gameplayCanvas.gameObject.SetActive(false);
    shopCanvas.gameObject.SetActive(true);

    if (SoundManager.Instance != null)
    {
        SoundManager.Instance.FadeToShopAmbience();
    }

    shopManager.OpenShop();
}
}

