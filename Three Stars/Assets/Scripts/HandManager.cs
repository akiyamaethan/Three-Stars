using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;
using UnityEngine.UI;
using System.Collections;

public class HandManager : MonoBehaviour
{
    //Managers
    [HideInInspector] public DeckManager deckManager;
    [HideInInspector] public HandEvaluator handEvaluator;

    //Prefabs
    public GameObject cardPrefab;
    public GameObject chefCardPrefab;
    public GameObject pipPopupPrefab;

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
    private List<GameObject> activeChefVisuals = new List<GameObject>();

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
    private IEnumerator PlayHandRoutine(List<CardMovement> cardsToMove, Transform[] foodTargets, List<float> cardPips, int finalScore, List<CardInstance> cardsToScore)
    {
        for (int i = 0; i < foodTargets.Length; i++)
        {
            publicDiscards.Add(cardsToMove[i].gameObject);
            cardsToMove[i].PlayFancyAnimation(foodTargets[i], cardPips[i], pipPopupPrefab);
            cardsInHand.Remove(cardsToMove[i].gameObject);
            yield return new WaitForSeconds(0.3f);
        }
        selectedCards.Clear();

        yield return new WaitForSeconds(3.0f);

        OnHandPlayed.Invoke(cardsToScore, finalScore);
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
        foreach (GameObject chef in activeChefVisuals)
        {
            Destroy(chef);
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

    // Updaters

    private void UpdateChefPositions()
    {
        foreach (GameObject chefGO in activeChefVisuals)
        {
            Destroy(chefGO);
        }
        activeChefVisuals.Clear();

        var activeChefs = GameManager.Instance.progressionManager.activeChefs;
        foreach (var chef in activeChefs)
        {
            GameObject chefGO = Instantiate(chefCardPrefab, handTransform);
            ChefCardDisplay display = chefGO.GetComponent<ChefCardDisplay>();

            if (display != null)
            {
                display.cardData = chef.data;
                display.UpdateCardDisplay(chef.remainingShifts);
            }

            activeChefVisuals.Add(chefGO);
        }
    }
    private void UpdateCardPositions()
    {
        UpdateChefPositions();

        int cardCount = cardsInHand.Count;
        int chefCount = activeChefVisuals.Count;
        if (cardCount == 0) return;

        float maxWidth = 1600f;
        float defaultSpacing = 250f;

        float chefAreaWidth = chefCount * defaultSpacing;
        float availableHandSpace = maxWidth - chefAreaWidth;
        float currentSpacing = defaultSpacing;

        if (cardCount > 1)
        {
            float totalNeededWidth = (cardCount - 1) * defaultSpacing;
            if (totalNeededWidth > availableHandSpace) currentSpacing = availableHandSpace / (cardCount - 1);
        }

        float totalWidth = chefAreaWidth + (cardCount > 0 ? (cardCount - 1) * currentSpacing : 0);
        float startX = (-totalWidth / 2);

        // Position chef cards
        for (int i = 0; i < chefCount; i++)
        {
            Vector3 pos = new Vector3(startX + (i * defaultSpacing) , 0, 0);
            activeChefVisuals[i].transform.localPosition = pos;
        }

        // Position normal cards
        float cardStartX = startX + chefAreaWidth;
        for (int i = 0; i < cardCount; i++)
        {
            Vector3 targetPosition = new Vector3(cardStartX + (i * currentSpacing), 0, 0);
            cardsInHand[i].transform.localPosition = targetPosition;
            // the rightmost card is rendered last
            cardsInHand[i].transform.SetAsLastSibling();

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
        List<float> individualPips = new List<float>();
        List<CardMovement> cardsToMove = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject)); //this is the list of cards on the canvas

        foreach (CardMovement card in selectedCards)
        {
            var instance = card.GetComponent<CardDisplay>().cardInstance;
            cardsToScore.Add(instance);
            individualPips.Add(GameManager.Instance.scoreManager.GetCardPips(instance));
        }

        HandEvaluator.HandRank handRank;
        int finalScore = GameManager.Instance.scoreManager.CalculateScore(cardsToScore, out handRank);

        //visually moves card to discard pile
        Transform[] foodTargets = {foodTransform1, foodTransform2, foodTransform3, foodTransform4};
        StartCoroutine(PlayHandRoutine(cardsToMove, foodTargets, individualPips, finalScore, cardsToScore));


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
