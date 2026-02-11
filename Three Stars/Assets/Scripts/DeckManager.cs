using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class DeckManager : MonoBehaviour
{
    public List<PlayingCard> deck = new List<PlayingCard>();
    private int currentCardIndex = 0;

    public void Start()
    {
        //Load all card assets from the Resources folder and add them to the deck
        PlayingCard[] allCards = Resources.LoadAll<PlayingCard>("Playing Card Data");
        deck.AddRange(allCards);
    }
    public void DrawCard(HandManager handManager)
    {
        if (deck.Count == 0)
            return;

        PlayingCard cardToDraw = deck[currentCardIndex];
        handManager.AddCardToHand(cardToDraw);
        currentCardIndex++;
    }

    public void Shuffle()
    {

    }

}
