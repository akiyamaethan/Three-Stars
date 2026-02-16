using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DeckManager deckManager { get; private set; }
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
    }
    
    private int _playerScore;
    public int PlayerScore
    {
        get { return _playerScore; }
        set { _playerScore = value; }
    }
}

