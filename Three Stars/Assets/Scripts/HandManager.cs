using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HandManager : MonoBehaviour
{
    //
    // Hand Manager: Mainly manages hand visuals but also keeps references for the discard pile for other managers to use.
    // Manages: hand representation on screen (including chef cards), sorting the hand, scoring animation, playing/discarding
    // cards, adding/removing cards from the hand
    //

    //Managers
    [HideInInspector] public DeckManager deckManager;
    [HideInInspector] public HandEvaluator handEvaluator;

    //Prefabs
    public GameObject cardPrefab;
    public GameObject chefCardPrefab;
    [Header("Scoring Animation Prefabs")]
    public GameObject pipPopupPrefab;
    public GameObject outerPanelPrefab;
    public GameObject innerPanelPrefab;
    public GameObject handDisplayPrefab;
    public GameObject multDisplayPrefab;

    //Plate Animation Settings
    public Transform platesParent;
    public float platesOffScreenY = 1200f;
    public float platesOnScreenY = 70.13f;
    public float platesMoveDuration = 0.5f;

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

    [Header("Dish Score UI")]
    public TextMeshProUGUI HandScoreText;
    public GameObject HandScorePanel;
    public CanvasGroup HandScoreCanvas;
    public float fadeDuration = 5f;

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
        ClearHand();
        SetHighlightDiscardButton(false);
        SetHighlightPlayButton(false);
    }

    // Coroutines
    private IEnumerator PlayHandRoutine(List<CardMovement> cardsToMove, Transform[] foodTargets, List<float> cardPips, int finalScore, List<CardInstance> cardsToScore)
    {
        //Disable play and discard buttons
        SetHighlightDiscardButton(false);
        SetHighlightPlayButton(false);

        //Lerp plate onto screen
        yield return StartCoroutine(LerpPlates(true));

        //Underneath the plate - enable the hand score display and set its value
        if (HandScoreCanvas != null)
        {
            HandScoreCanvas.alpha = 1f;
        }
        if (HandScoreText != null)
            HandScoreText.text = $"Dish Score: {finalScore}";

        //Initialize variables for scoring animation
        HandEvaluator.HandRank rank = GameManager.Instance.scoreManager.handEvaluator.EvaluateHand(cardsToScore);
        float multiplier = GameManager.Instance.scoreManager.GetTotalMult(cardsToScore, rank);
        float runningPipTotal = 0;

        //Reset score X mult UI component
        UIManager.instance.UpdateScoreXMultMult(0f);
        UIManager.instance.UpdateScoreXMultScore(0f);

        //Foreach selected card, start its animation, add its pips to running total, and wait .03s
        for (int i = 0; i < foodTargets.Length; i++)
        {
            publicDiscards.Add(cardsToMove[i].gameObject);
            cardsToMove[i].PlayFancyAnimation(foodTargets[i], cardPips[i], pipPopupPrefab);
            cardsInHand.Remove(cardsToMove[i].gameObject);

            runningPipTotal += cardPips[i];
            UIManager.instance.UpdateScoreXMultScore(runningPipTotal);

            yield return new WaitForSeconds(0.3f);
        }
        //Clear selected cards, wait 3s
        selectedCards.Clear();
        yield return new WaitForSeconds(3f);

        //Activate panel animation
        yield return StartCoroutine(AnimateScoringPanels(rank, multiplier));

        //Lerp plate off screen and fade score text
        StartCoroutine(LerpPlates(false));
        StartCoroutine(FadeDishScore());

        //Update UI, notify other managers a hand was played, and reset the hand
        UIManager.instance.UpdateScoreXMultMult(multiplier);
        OnHandPlayed.Invoke(cardsToScore, finalScore);
        DrawToFullHand();
    }

    private IEnumerator AnimateScoringPanels(HandEvaluator.HandRank rank, float mult)
    {
        Canvas canvas = GameManager.Instance.gameplayCanvas;

        GameObject outer = Instantiate(outerPanelPrefab, canvas.transform);
        GameObject inner = Instantiate(innerPanelPrefab, canvas.transform);
        GameObject handObj = Instantiate(handDisplayPrefab, canvas.transform);
        GameObject multObj = Instantiate(multDisplayPrefab, canvas.transform);

        RectTransform outerRT = outer.GetComponent<RectTransform>();
        RectTransform innerRT = inner.GetComponent<RectTransform>();
        RectTransform handRT = handObj.GetComponent<RectTransform>();
        RectTransform multRT = multObj.GetComponent<RectTransform>();

        handObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = GetThemedHandName(rank);
        multObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Multiplier: {mult}";

        float slideDuration = 0.2f;
        Vector2 centerPos = Vector2.zero;
        Vector2 handTargetPos = new Vector2 (0, 50);
        Vector2 multTargetPos = new Vector2 (0, -50);

        float screenWidth = Screen.width;
        Vector2 leftPanelPos = new Vector2(-screenWidth, 0);
        Vector2 leftHandPos = new Vector2(-screenWidth, handTargetPos.y);
        Vector2 leftMultPos = new Vector2 (-screenWidth, multTargetPos.y);

        outerRT.position = leftPanelPos;
        innerRT.position = leftPanelPos;
        handRT.position = leftHandPos;
        multRT.position = leftMultPos;

        StartCoroutine(LerpAnchoredPosition(outerRT, centerPos, slideDuration));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LerpAnchoredPosition(innerRT, centerPos, slideDuration));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LerpAnchoredPosition(handRT, handTargetPos, slideDuration));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LerpAnchoredPosition(multRT, multTargetPos, slideDuration));
        yield return new WaitForSeconds(2.1f);

        StartCoroutine(LerpAnchoredPosition(outerRT, leftPanelPos, slideDuration));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LerpAnchoredPosition(innerRT, leftPanelPos, slideDuration));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LerpAnchoredPosition(handRT, leftHandPos, slideDuration));
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LerpAnchoredPosition(multRT, leftMultPos, slideDuration));
        yield return new WaitForSeconds(0.1f);

        Destroy(inner);
        Destroy(outer);
        Destroy(handObj);
        Destroy(multObj);
    }

    private IEnumerator LerpAnchoredPosition(RectTransform target, Vector2 targetPos, float duration)
    {
        Vector2 startPos = target.anchoredPosition;
        float elapsed = 0f;
        while ( elapsed < duration)
        {
            if (target)
                target.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (target)
            target.anchoredPosition = targetPos;
    }

    private IEnumerator LerpPlates(bool moveOnScreen)
    {
        float targetY = moveOnScreen ? platesOnScreenY : platesOffScreenY;
        Vector3 startPos = platesParent.localPosition;
        Vector3 endPos = new Vector3(startPos.x, targetY, startPos.z);
        float elapsed = 0f;

        while (elapsed < platesMoveDuration)
        {
            platesParent.localPosition = Vector3.Lerp(startPos, endPos, elapsed / platesMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        platesParent.localPosition = endPos;
    }

    private IEnumerator FadeDishScore()
    {
        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (HandScoreCanvas)
                HandScoreCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (HandScoreCanvas)
            HandScoreCanvas.alpha = endAlpha;
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

    if (SoundManager.Instance != null)
        SoundManager.Instance.PlayHand();

    foreach (GameObject card in publicDiscards)
    {
        if (card != null) Destroy(card);
    }
    publicDiscards.Clear();

    List<CardInstance> cardsToScore = new List<CardInstance>();
    List<float> individualPips = new List<float>();
    List<CardMovement> cardsToMove = selectedCards.FindAll(card => cardsInHand.Contains(card.gameObject));

    foreach (CardMovement card in selectedCards)
    {
        var instance = card.GetComponent<CardDisplay>().cardInstance;
        cardsToScore.Add(instance);
        individualPips.Add(GameManager.Instance.scoreManager.GetCardPips(instance));
    }

    HandEvaluator.HandRank handRank;
    int finalScore = GameManager.Instance.scoreManager.CalculateScore(cardsToScore, out handRank);

    Transform[] foodTargets = { foodTransform1, foodTransform2, foodTransform3, foodTransform4 };
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
        if (SoundManager.Instance != null) SoundManager.Instance.PlayDiscard();

        selectedCards.Clear();
        UpdateButtons();
        DrawToFullHand();
    }

    public void SortByRank()
{
    if (SoundManager.Instance != null)
        SoundManager.Instance.PlayRankSuitClick();

    cardsInHand = cardsInHand
        .OrderBy(card => card.GetComponent<CardDisplay>().cardInstance.cardData.cardRank)
        .ToList();

    UpdateCardPositions();
    sortPreference = SortType.Rank;
    sortRankButton.interactable = false;
    sortSuitButton.interactable = true;
}

public void SortBySuit()
{
    if (SoundManager.Instance != null)
        SoundManager.Instance.PlayRankSuitClick();

    cardsInHand = cardsInHand
        .OrderBy(card => card.GetComponent<CardDisplay>().cardInstance.cardData.cardSuit)
        .ToList();

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

    // Misc
    private string GetThemedHandName(HandEvaluator.HandRank rank)
    {
        return rank switch
        { 
            HandEvaluator.HandRank.HighCard => "A La Carte",
            HandEvaluator.HandRank.Rainbow => "Balanced Meal",
            HandEvaluator.HandRank.OnePair => "Pairing",
            HandEvaluator.HandRank.TwoPair => "Split Plate",
            HandEvaluator.HandRank.ThreeOfAKind => "Set",
            HandEvaluator.HandRank.Straight => "Chef's Sequence",
            HandEvaluator.HandRank.Flush => "Single Origin",
            HandEvaluator.HandRank.FourOfAKind => "Perfect Meal",
            HandEvaluator.HandRank.StraightFlush => "Single Origin Sequence",
            HandEvaluator.HandRank.RoyalFlush => "Grand Buffet",
            _ => "Specialty"
        };
    }
}
