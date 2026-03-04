using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;
using UnityEngine.UI;
using System.Collections;

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
    public List<GameObject> publicDiscards = new List<GameObject>(); // Used for visuals
    public List<GameObject> privateDiscards = new List<GameObject>(); // USE THIS FOR DISCARD MODAL

    //Events
    public static event System.Action<List<CardInstance>, int> OnHandPlayed;

    // Misc
    private int redealCount = 0;

    //Enums
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

    // Coroutines
    private IEnumerator PlayHandRoutine(List<CardMovement> cardsToMove, Transform[] foodTargets)
    {
        for (int i = 0; i < foodTargets.Length; i++)
        {
            publicDiscards.Add(cardsToMove[i].gameObject);
            cardsToMove[i].PlayFancyAnimation(foodTargets[i]);
            cardsInHand.Remove(cardsToMove[i].gameObject);
        }
        selectedCards.Clear();

        yield return new WaitForSeconds(3.0f);
        DrawToFullHand();
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
        int cardsBeforeDrawCount = cardsInHand.Count;
        int targetHandSize = GameManager.Instance.progressionManager.handSize;
        // Draw the cards
        int iterations = 0; //infinit loop protection
        int maxIterations = 100;
        while (cardsInHand.Count < targetHandSize && iterations < maxIterations)
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

        int cardsDrawnCount = cardsInHand.Count - cardsBeforeDrawCount;
        if (cardsDrawnCount <= 0) return;
        bool triggerRedeal = false;

        foreach (var chef in GameManager.Instance.progressionManager.activeChefs)
        {
            if (chef.data.cardName == "Chef Matt")
            {
                var newCards = cardsInHand.GetRange(cardsBeforeDrawCount, cardsDrawnCount);
                if (!newCards.Any(c => c.GetComponent<CardDisplay>().cardInstance.cardData.cardRank == PlayingCard.CardRank.Ace))
                {
                    triggerRedeal = true;
                }
            }
            else if (chef.data.cardName == "Chef Ryan")
            {
                var newCards = cardsInHand.GetRange(cardsBeforeDrawCount, cardsDrawnCount);
                if (!newCards.Any(c => c.GetComponent<CardDisplay>().cardInstance.cardData.cardRank == PlayingCard.CardRank.King))
                {
                    triggerRedeal = true;
                }
            }
        }

        if (triggerRedeal && redealCount < 2)
        {
            redealCount++;
            for (int i = 0; i < cardsDrawnCount; i++)
            {
                int lastIndex = cardsInHand.Count - 1;
                GameObject cardToDestroy = cardsInHand[lastIndex];
                cardsInHand.RemoveAt(lastIndex);
                Destroy(cardToDestroy);
            }
            deckManager.RewindAndPartialShuffle(cardsDrawnCount);

            DrawToFullHand();
            return;
        }
        redealCount = 0;
        UpdateCardPositions();
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
        foreach (GameObject card in publicDiscards)
        {
            Destroy(card);
        }
        foreach (GameObject card in privateDiscards)
        {
            Destroy(card);
        }

        cardsInHand.Clear();
        selectedCards.Clear();
        publicDiscards.Clear();
        privateDiscards.Clear();

        if (GameManager.Instance != null && GameManager.Instance.DiscardPileTransform != null)
        {
            Image discardPileImage = GameManager.Instance.DiscardPileTransform.GetComponentInChildren<Image>(true);
            if (discardPileImage != null)
            {
                discardPileImage.gameObject.SetActive(false);
            }
        }
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
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        float maxWidth = 1600f;
        float defaultSpacing = 250f;
        float chefAreaWidth = GameManager.Instance.progressionManager.activeChefs.Count * defaultSpacing;
        float currentSpacing = defaultSpacing;
        float totalNeededWidth = (cardCount - 1) * defaultSpacing;
        float availableHandSpace = maxWidth - chefAreaWidth;

        if (totalNeededWidth > availableHandSpace) currentSpacing = availableHandSpace / (cardCount - 1);

      
        float actualWidth = (cardCount - 1) * currentSpacing;
        float startX = (-actualWidth / 2) + (chefAreaWidth /2);

        for (int i = 0; i < cardCount; i++)
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
        if (selectedCards.Count != 4 || GameManager.Instance.shiftManager.plays < 1) return;

        foreach(GameObject card in publicDiscards)
        {
            if (card != null) Destroy(card);
        }
        publicDiscards.Clear();

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
        Transform[] foodTargets = {foodTransform1, foodTransform2, foodTransform3, foodTransform4};
        StartCoroutine(PlayHandRoutine(cardsToMove, foodTargets));


    }

    public void OnDiscardButtonPressed()
    {
        if (selectedCards.Count > 4 || selectedCards.Count == 0) return;
        if (GameManager.Instance.shiftManager.discards < 1) return;

        GameManager.Instance.shiftManager.TriggerDiscard();
        List<CardMovement> cardsToDiscard = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject));

        cardsToDiscard.ForEach(card =>
        {
            privateDiscards.Add(card.gameObject);
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
