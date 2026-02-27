using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    //Managers
    public DeckManager deckManager;
    public GameObject cardPrefab; 
    public HandEvaluator handEvaluator;

    //Visual Variables
    public Transform handTransform;
    public Transform foodTransform1;
    public Transform foodTransform2;
    public Transform foodTransform3;
    public Transform foodTransform4;
    public float cardSpacing = 150f;
    public Button playButton = null;
    public Button discardButton = null;
    public Button sortRankButton = null;
    public Button sortSuitButton = null;
    public SortType sortPreference = SortType.Rank;

    //Card Trackers
    public List<GameObject> cardsInHand = new List<GameObject>();
    public List<CardMovement> selectedCards = new List<CardMovement>();
    public List<GameObject> discards = new List<GameObject>();

    //Events
    public static event System.Action<List<CardInstance>, int> OnHandPlayed;

    //Enum
    public enum SortType
    {
        Rank,
        Suit
    }

    public void Initialize(DeckManager dm, HandEvaluator he)
    {
        this.deckManager = dm;
        this.handEvaluator = he;
        SetHighlightDiscardButton(false);
        SetHighlightPlayButton(false);
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
        // Draw the cards
        int iterations = 0; //infinit loop protection
        int maxIterations = 100;
        while (cardsInHand.Count < GameManager.Instance.progressionManager.handSize && iterations < maxIterations)
        {
            int countBefore = cardsInHand.Count;
            deckManager.DrawCard(this);
            iterations++;

            if (cardsInHand.Count == countBefore)
            {
                Debug.LogWarning("No more cards to draw, stopping draw loop.");
                break;
            }
        }

        // Sort according to preference
        if (sortPreference == SortType.Rank)
        {
            SortByRank();
        }
        else
        {
            SortBySuit();
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
        UpdateButtons();
    }

    public void UpdateButtons()
    {

        if (selectedCards.Count < 5 && selectedCards.Count > 0)
        {
            SetHighlightDiscardButton(true);
        }
        else
        {
            SetHighlightDiscardButton(false);
        }
        if (selectedCards.Count == 4)
        {
            SetHighlightPlayButton(true);
        }
        else
        {
            SetHighlightPlayButton(false);
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
        sortPreference = SortType.Rank;
        sortRankButton.interactable = false;
        sortSuitButton.interactable = true;
    }

    public void SortBySuit()
    {
        cardsInHand = cardsInHand.OrderBy(card => card.GetComponent<CardDisplay>().cardInstance.cardData.cardSuit).ToList();
        UpdateCardPositions();
        sortPreference = SortType.Suit;
        sortRankButton.interactable = true;
        sortSuitButton.interactable = false;
    }

    public void SetHighlightPlayButton(bool highlight)
    {
        if (highlight)
        {
            playButton.interactable = true;
        }
        else
        {
            playButton.interactable = false;
        }
    }

    public void SetHighlightDiscardButton(bool highlight)
    {
        if (highlight)
        {
            discardButton.interactable = true;
        }
        else
        {
            discardButton.interactable = false;
        }
    }
}
