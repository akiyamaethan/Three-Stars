using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Transform[] handSlots;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SelectionManager selectionManager;

    private readonly List<GameObject> spawnedCards = new();

    public void RenderHandSlots(CardData[] slotData)
    {
        // Clear all existing spawned cards
        foreach (var go in spawnedCards) Destroy(go);
        spawnedCards.Clear();

        int count = Mathf.Min(slotData.Length, handSlots.Length);

        for (int i = 0; i < count; i++)
        {
            var data = slotData[i];
            if (data == null) continue; // leave slot empty

            var slot = handSlots[i];

            var go = Instantiate(cardPrefab, slot);
            spawnedCards.Add(go);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var view = go.GetComponent<CardView>();
            if (view == null) view = go.AddComponent<CardView>();
            view.Init(data, ShortName(data));

            var selectable = go.GetComponent<SelectableCard>();
            if (selectable == null) selectable = go.AddComponent<SelectableCard>();
            selectable.Init(selectionManager);
        }
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
