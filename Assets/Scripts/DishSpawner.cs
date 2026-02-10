using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DishSpawner : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private MenuDatabase menuDatabase;
    [SerializeField] private Transform plateAnchor;
    [SerializeField] private GameObject dishPrefab;

    private GameObject currentDish;

    private void OnEnable()
    {
        if (selectionManager != null)
        {
            selectionManager.OnPlayedCards += HandlePlayedCards;
        }
    }

    private void OnDisable()
    {
        if (selectionManager != null)
        {
            selectionManager.OnPlayedCards -= HandlePlayedCards;
        }
    }

    private void HandlePlayedCards(List<CardData> playedCards)
    {
        if (playedCards == null || playedCards.Count == 0) return;
        if (menuDatabase == null) return;
        if (plateAnchor == null) return;
        if (dishPrefab == null) return;

        // Build lists per suit (Suit decides category)
        var spades = new List<CardData>();
        var hearts = new List<CardData>();
        var diamonds = new List<CardData>();
        var clubs = new List<CardData>();

        foreach (var c in playedCards)
        {
            if (c == null) continue;

            switch (c.Suit)
            {
                case Suit.Spades: spades.Add(c); break;     // Entree
                case Suit.Hearts: hearts.Add(c); break;     // Side
                case Suit.Diamonds: diamonds.Add(c); break; // Vegetable
                case Suit.Clubs: clubs.Add(c); break;       // Sauce
            }
        }

        // Sort each suit list high-to-low so better items are listed first
        spades.Sort((a, b) => ((int)b.Rank).CompareTo((int)a.Rank));
        hearts.Sort((a, b) => ((int)b.Rank).CompareTo((int)a.Rank));
        diamonds.Sort((a, b) => ((int)b.Rank).CompareTo((int)a.Rank));
        clubs.Sort((a, b) => ((int)b.Rank).CompareTo((int)a.Rank));

        // Replace previous dish UI
        if (currentDish != null) Destroy(currentDish);
        currentDish = Instantiate(dishPrefab, plateAnchor, false);

        // Fill the anchor
        var rt = currentDish.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // Write dish text
        var label = currentDish.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            label.text = BuildDishText(spades, hearts, diamonds, clubs);
        }
    }

    private string BuildDishText(
        List<CardData> spades,
        List<CardData> hearts,
        List<CardData> diamonds,
        List<CardData> clubs)
    {
        var lines = new List<string>();

        // Spades -> Entree
        if (spades.Count > 0)
        {
            lines.Add("Entree:");
            foreach (var c in spades)
            {
                lines.Add($"- {menuDatabase.GetEntree(c.Rank)} ({ShortName(c)})");
            }
        }

        // Hearts -> Side
        if (hearts.Count > 0)
        {
            lines.Add("Side:");
            foreach (var c in hearts)
            {
                lines.Add($"- {menuDatabase.GetSide(c.Rank)} ({ShortName(c)})");
            }
        }

        // Diamonds -> Vegetable
        if (diamonds.Count > 0)
        {
            lines.Add("Vegetable:");
            foreach (var c in diamonds)
            {
                lines.Add($"- {menuDatabase.GetVegetable(c.Rank)} ({ShortName(c)})");
            }
        }

        // Clubs -> Sauce
        if (clubs.Count > 0)
        {
            lines.Add("Sauce:");
            foreach (var c in clubs)
            {
                lines.Add($"- {menuDatabase.GetSauce(c.Rank)} ({ShortName(c)})");
            }
        }

        // If only one suit was played, only that section appears.
        return string.Join("\n", lines);
    }

    private string ShortName(CardData c)
    {
        string r = c.Rank switch
        {
            Rank.Ace => "A",
            Rank.King => "K",
            Rank.Queen => "Q",
            Rank.Jack => "J",
            Rank.Ten => "10",
            Rank.Nine => "9",
            Rank.Eight => "8",
            Rank.Seven => "7",
            Rank.Six => "6",
            Rank.Five => "5",
            Rank.Four => "4",
            Rank.Three => "3",
            Rank.Two => "2",
            _ => "?"
        };

        string s = c.Suit switch
        {
            Suit.Spades => "S",
            Suit.Hearts => "H",
            Suit.Diamonds => "D",
            Suit.Clubs => "C",
            _ => "?"
        };

        return r + s;
    }
}