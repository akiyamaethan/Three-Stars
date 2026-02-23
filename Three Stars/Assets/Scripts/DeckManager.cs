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
        //Load all card assets from the Resources folder and add them to the deck
        PlayingCard[] allCards = Resources.LoadAll<PlayingCard>("Playing Card Data");
        for (int i = 0; i < allCards.Length; i++)
        {
            CardInstance cardInstance = new CardInstance(allCards[i]);
            deck.Add(cardInstance);
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
