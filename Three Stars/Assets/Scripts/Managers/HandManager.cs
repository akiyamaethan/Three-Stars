using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class HandManager : MonoBehaviour
{
    //Managers
    public DeckManager deckManager; // Assign in inspector
    public GameObject cardPrefab; // Assign in inspector
    public HandEvaluator handEvaluator; // Assign in inspector

    //Visual Variables
    public Transform handTransform;
    public float cardSpacing = 150f;

    //Card Trackers
    public List<GameObject> cardsInHand = new List<GameObject>();
    public List<CardMovement> selectedCards = new List<CardMovement>();
    public List<GameObject> discards = new List<GameObject>();

    //Events
    public static event System.Action<List<CardInstance>, int> OnHandPlayed;

    private void Awake()
    {
        deckManager = FindAnyObjectByType<DeckManager>();
    }


    // Hand Manipulation Methods
    public void AddCardToHand(CardInstance cardInstance)
    {
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);

        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardInstance = cardInstance;
        cardDisplay.UpdateCardDisplay();

        UpdateCardPositions();
    }
    public void DrawToFullHand()
    {
        if (GameManager.Instance == null || GameManager.Instance.progressionManager == null)
        {
            Debug.LogWarning("HandManager: Cannot draw cards, ProgressionManager is not initialized.");
            return;
        }

        if (deckManager == null)
        {
            Debug.LogWarning("HandManager: deckManager is null!");
            return;
        }

        while (cardsInHand.Count < GameManager.Instance.progressionManager.handSize)
        {
            deckManager.DrawCard(this);
        }
    }

    public void ClearHand()
    {
        foreach (GameObject card in cardsInHand)
        {
            Destroy(card);
        }
        foreach (GameObject card in discards)
        {
            Destroy(card);
        }
        cardsInHand.Clear();
        selectedCards.Clear();
        discards.Clear();
    }

    // Selection Methods
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

    // Updater
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

    // Event Handlers
    public void OnPlayButtonPressed()
    {
        if (selectedCards.Count != 4)
            return;
        if (GameManager.Instance.shiftManager.plays < 1)
            return;

        // Cleanup previous discards first
        foreach (GameObject oldCard in discards)
        {
            if (oldCard != null) Destroy(oldCard);
        }
        discards.Clear();

        List<CardInstance> cardsToScore = new List<CardInstance>(); //this gets passed to the score manager
        List<CardMovement> cardsToMove = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject)); //this is the list of cards on the canvas
        foreach (CardMovement card in selectedCards)
        {
            cardsToScore.Add(card.GetComponent<CardDisplay>().cardInstance);
        }
        HandEvaluator.HandRank handRank;
        int finalScore = GameManager.Instance.scoreManager.CalculateScore(cardsToScore, out handRank);

        OnHandPlayed?.Invoke(cardsToScore, finalScore);

        Debug.Log($"Played hand with rank {handRank} for {finalScore} points!");

        //visually moves card to discard pile
        cardsToMove.ForEach(card =>
        {
            discards.Add(card.gameObject);
            card.Play();
            cardsInHand.Remove(card.gameObject);
        });

        //Reset hand selection and card amount
        selectedCards.Clear();
        DrawToFullHand();
    }

    public void OnDiscardButtonPressed()
    {
        if (selectedCards.Count > 4 || selectedCards.Count == 0)
            return;
        if (GameManager.Instance.shiftManager.discards < 1)
            return;

        // Cleanup previous discards first
        foreach (GameObject oldCard in discards)
        {
            if (oldCard != null) Destroy(oldCard);
        }
        discards.Clear();

        GameManager.Instance.shiftManager.TriggerDiscard();
        List<CardMovement> cardsToDiscard = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject));
        cardsToDiscard.ForEach(card =>
        {
            discards.Add(card.gameObject);
            card.Discard();
            cardsInHand.Remove(card.gameObject);
        });
        selectedCards.Clear();
        DrawToFullHand();
    }

    public void SortByRank()
    {
        cardsInHand = cardsInHand.OrderBy(card => card.GetComponent<CardDisplay>().cardInstance.cardData.cardRank).ToList();
        UpdateCardPositions();
    }

    public void SortBySuit()
    {
        cardsInHand = cardsInHand.OrderBy(card => card.GetComponent<CardDisplay>().cardInstance.cardData.cardSuit).ToList();
        UpdateCardPositions();
    }
}
