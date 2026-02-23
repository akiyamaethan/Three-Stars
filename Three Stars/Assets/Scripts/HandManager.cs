using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;
using Unity.VisualScripting;

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
        if (selectedCards.Count != 4)
            return;
        if (ShiftManager.Instance.plays < 1)
            return;

        List<CardInstance> cardsToScore = new List<CardInstance>(); //this gets passed to the score manager
        List<CardMovement> cardsToMove = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject)); //this is the list of cards on the canvas
        foreach (CardMovement card in selectedCards)
        {
            cardsToScore.Add(card.GetComponent<CardDisplay>().cardInstance);
        }
        HandEvaluator.HandRank handRank;
        int finalScore = ScoreManager.Instance.CalculateScore(cardsToScore, out handRank);

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
        if (ShiftManager.Instance.discards < 1)
            return;
        ShiftManager.Instance.TriggerDiscard();
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
    public void DrawToFullHand()
    {
        while (cardsInHand.Count < ProgressionManager.Instance.handSize)
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
}
