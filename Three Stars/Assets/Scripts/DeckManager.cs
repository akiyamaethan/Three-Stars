using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class DeckManager : MonoBehaviour
{
    public List<CardInstance> deck = new List<CardInstance>();
    private int currentCardIndex = 0;

    public void Awake()
    {
        //Load all card assets from the Resources folder and add them to the deck
        PlayingCard[] allCards = Resources.LoadAll<PlayingCard>("Playing Card Data");
        for (int i = 0; i < allCards.Length; i++)
        {
            CardInstance cardInstance = new CardInstance(allCards[i]);
            deck.Add(cardInstance);
        }
    }
    public void DrawCard(HandManager handManager)
    {
        if (deck.Count == 0)
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

    }

}
