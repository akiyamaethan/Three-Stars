using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class HandManager : MonoBehaviour
{
    public DeckManager deckManager; // Assign in inspector
    public GameObject cardPrefab; // Assign in inspector
    public Transform handTransform;
    public float cardSpacing = 250f;
    public List<GameObject> cardsInHand = new List<GameObject>();
    void Start()
    {
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);

    }

    public void AddCardToHand(PlayingCard cardData)
    {
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardData = cardData;
        cardDisplay.UpdateCardDisplay();

        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        
        float totalWidth = (cardsInHand.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector3 targetPosition = new Vector3(startX + i * cardSpacing, 0, 0);
            cardsInHand[i].transform.localPosition = targetPosition;
        }
    }


}
