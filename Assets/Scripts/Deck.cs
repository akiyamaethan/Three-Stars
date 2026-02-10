using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<CardData> _cards = new List<CardData>();

    public int Count => _cards.Count;

    public void Build52()
    {
        _cards.Clear();

        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
            {
                _cards.Add(new CardData(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        // Fisher-Yates shuffle
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public CardData Draw()
    {
        if (_cards.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        CardData top = _cards[_cards.Count - 1];
        _cards.RemoveAt(_cards.Count - 1);
        return top;
    }
}
