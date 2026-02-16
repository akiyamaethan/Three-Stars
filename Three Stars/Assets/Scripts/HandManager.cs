using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;
using Unity.VisualScripting;

public class HandManager : MonoBehaviour
{
    public DeckManager deckManager; // Assign in inspector
    public GameObject cardPrefab; // Assign in inspector
    public Transform handTransform;
    public float cardSpacing = 150f;
    public List<GameObject> cardsInHand = new List<GameObject>();
    public List<CardMovement> selectedCards = new List<CardMovement>();
    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);
        deckManager.DrawCard(this);

    }

    public void AddCardToHand(CardInstance cardInstance)
    {
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardInstance = cardInstance;
        cardDisplay.UpdateCardDisplay();

        UpdateCardPositions();
    }

    public void SetSelected (CardMovement card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
        }
        else
        {
            selectedCards.Add(card);
        }
    }

    private void UpdateCardPositions()
    {
        
        float totalWidth = (cardsInHand.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector3 targetPosition = new Vector3(startX + i * cardSpacing, 0, 0);
            cardsInHand[i].transform.localPosition = targetPosition;

            CardMovement cardMovement = cardsInHand[i].GetComponent<CardMovement>();
            if (cardMovement != null)
            {
                cardMovement.SetOriginalPosition(targetPosition);
            }
        }
    }

    public void OnPlayButtonPressed()
    {
        List<CardMovement> cardsToPlay = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject));
        cardsToPlay.ForEach(card =>
        {
            card.Play();
            cardsInHand.Remove(card.gameObject);
        });
        selectedCards.Clear();
    }

    public void OnDiscardButtonPressed()
    {
        List<CardMovement> cardsToDiscard = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject));
        cardsToDiscard.ForEach(card =>
        {
            card.Play();
            cardsInHand.Remove(card.gameObject);
        });
        selectedCards.Clear();
    }


}
