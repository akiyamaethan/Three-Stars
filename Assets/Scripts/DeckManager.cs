using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class DeckManager : MonoBehaviour
{
    public List<CardInstance> deck = new List<CardInstance>();
    private int currentCardIndex = 0;
    private static System.Random _rng = new System.Random();

    public void Awake()
    {
        if (deck.Count == 0)
        {
            InitializeDeck();
        }
    }

    public void InitializeDeck()
    {
        deck.Clear();
        PlayingCard[] allCards = Resources.LoadAll<PlayingCard>("Playing Card Data");
        foreach (PlayingCard card in allCards)
        {
            deck.Add(new CardInstance(card));
        }
        Shuffle();
    }
    public void DrawCard(HandManager handManager)
    {
        if (currentCardIndex >= deck.Count)
        {
            Debug.LogWarning("Deck is empty! Cannot draw a card.");
            return;
        }

        CardInstance cardToDraw = deck[currentCardIndex];
        handManager.AddCardToHand(cardToDraw);
        currentCardIndex++;
    }
    
    public void RewindAndPartialShuffle(int count)
    {
        if (count < 1) return;

        currentCardIndex -= count;
        if (currentCardIndex < 0) currentCardIndex = 0;

        int n = deck.Count;
        for (int i = currentCardIndex; i < n; i++)
        {
            int k = _rng.Next(i, n);

            CardInstance temp = deck[k];
            deck[k] = deck[i];
            deck[i] = temp;
        }
    }

    public void Shuffle()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            // Pick a random index from 0 to n
            int k = _rng.Next(n + 1);

            // Swap elements
            CardInstance value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
        currentCardIndex = 0;
    }

}
