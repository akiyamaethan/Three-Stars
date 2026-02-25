using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanelController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text targetText;

    [SerializeField] private Transform cardsContainer;
    [SerializeField] private TMP_Text cardLabelPrefab;

    [SerializeField] private Button goToShopButton;

    private void Awake()
    {
        if (goToShopButton != null)
            goToShopButton.onClick.AddListener(GoToShop);
    }

    public void Show(int score, int target, IEnumerable<string> linesToDisplay)
    {
        gameObject.SetActive(true);

        if (titleText != null) titleText.text = "Shift Complete";
        if (scoreText != null) scoreText.text = $"Score: {score}";
        if (targetText != null) targetText.text = $"Target: {target}";

        // Clear old items
        if (cardsContainer != null)
        {
            for (int i = cardsContainer.childCount - 1; i >= 0; i--)
                Destroy(cardsContainer.GetChild(i).gameObject);
        }

        // Spawn one label per line
        if (cardsContainer != null && cardLabelPrefab != null && linesToDisplay != null)
        {
            foreach (var line in linesToDisplay)
            {
                var label = Instantiate(cardLabelPrefab, cardsContainer);
                label.text = line;
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void GoToShop()
    {
        Hide();

        // Open shop using your existing state system
        GameManager.Instance.SwitchToState(GameManager.GameState.InShop);
        ShopManager.Instance.NotifyShopAvailable();
    }
}