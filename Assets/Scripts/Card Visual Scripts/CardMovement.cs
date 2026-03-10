using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardMovement : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    private RectTransform rectTransform; //Card Transform
    private RectTransform discardTransform; // Discard Pile
    private Vector3 originalScale;
    private Vector3 originalPosition; // These are used for selecting/making the card larger
    private int currentState = 0;

    // Animation variables
    private Quaternion originalRotation;
    private Coroutine animationCoroutine;
    private HandManager handManager;
    public CardDisplay cardDisplay;
    private GameObject foodToDestroy;
    private bool isFlipped = false;

    // Colors
    private Color hoverGlowColor = Color.grey;
    private Color selectedGlowColor = Color.white;

    // Parameters
    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] public Sprite cardBackSprite;

    void Awake()
    {
        discardTransform = GameManager.Instance.DiscardPileTransform;
        handManager = FindObjectOfType<HandManager>();
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
    }

    void Update()
    {
        switch (currentState)
        {
            case 0: // Idle
                HandleIdleState();
                break;
            case 1: // Hovered
                HandleHoverState();
                break;
            case 2: //Selected
                HandleSelectedState();
                break;
            case 3: // Played
                break;
        }
    }

    //Helpers
    private void TransitionToState0()
    {
        currentState = 0;
    }

    public void TransitionToPlayedState()
    {
        currentState = 3;
        glowEffect.SetActive(false);
    }
    public void SetOriginalPosition(Vector3 pos)
    {
        originalPosition = pos;
    }

    public void Discard()
    {
        TransitionToPlayedState();
        AnimateTo(discardTransform.position);
    }

    // Helper function to start the animation coroutine
    private void AnimateTo(Vector3 targetPosition)
    {
        if (!gameObject.activeInHierarchy)
        {
            transform.localPosition = targetPosition;
            return;
        }
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimatePosition(targetPosition));
    }

    private void SwapToFaceDown()
    {
        CardDisplay display = GetComponent<CardDisplay>();
        if (display != null && cardBackSprite != null)
        {
            display.cardBackground.sprite = cardBackSprite;
            display.cardBackground.color = Color.white;

            display.rankImage.gameObject.SetActive(false);
            display.suitImage.gameObject.SetActive(false);
            display.rankText.gameObject.SetActive(false);
            display.suitText.gameObject.SetActive(false);
            display.cardText.gameObject.SetActive(false);
        }
    }

    // Coroutines

    private IEnumerator AnimatePosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        isFlipped = false;

        // If we're already very close, just snap to the target
        if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            yield break;
        }

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);

            float rotationY = t * 180;
            transform.localRotation = Quaternion.Euler(0, rotationY, 0);

            if (t >= 0 && !isFlipped)
            {
                isFlipped = true;
                SwapToFaceDown();
            }

            yield return null;
        }
        transform.position = targetPosition;
        Image discardPileImage = discardTransform.GetComponentInChildren<Image>(true);
        if (discardPileImage != null)
        {
            discardPileImage.gameObject.SetActive(true);
        }
    }

    public void PlayFancyAnimation(Transform target, float pips, GameObject popupPrefab)
    {
        TransitionToPlayedState();
        StartCoroutine(FancyAnimation(target, pips, popupPrefab));
    }

    private IEnumerator FancyAnimation(Transform target, float pips, GameObject popupPrefab)
    {
        // Setup for scale up + popup
        cardDisplay = GetComponent<CardDisplay>();
        Canvas rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        Vector3 startPos = rectTransform.position;
        Vector3 peakPos = startPos + new Vector3(0, 10f, 0);
        Vector3 startScale = rectTransform.localScale;
        Vector3 peakScale = startScale * 1.1f;

        float popDuration = 1.3f;
        float elapsed = 0f;

        if (popupPrefab != null)
        {
            GameObject popup = Instantiate(popupPrefab, peakPos + new Vector3(0, 100f, 0 ), Quaternion.identity, rootCanvas.transform);
            popup.transform.localScale = Vector3.one;
            popup.transform.SetAsLastSibling();

            var popupText = popup.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (popupText != null) popupText.text = "+" + pips.ToString();

            Destroy(popup, 1.2f);
        }

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            float bounceT = Mathf.Sin(t * Mathf.PI);

            rectTransform.position = Vector3.Lerp(startPos, peakPos, bounceT);
            rectTransform.localScale = Vector3.Lerp(startScale, peakScale, bounceT);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Set up for image/card split
        Image foodImage = cardDisplay.foodImage;
        foodImage.transform.SetParent(rootCanvas.transform, true);
        foodToDestroy = foodImage.gameObject;

        Vector3 bodyStartPos = rectTransform.position;
        Vector3 bodyEndPos = discardTransform.position;
        Vector3 foodStartPos = foodImage.transform.position;

        float arcHeight = 800f;
        float moveDuration = 0.7f;
        isFlipped = false;
        elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            float easeT = t * t * (3f - 2f * t);

            Vector3 linearPos = Vector3.Lerp(bodyStartPos, bodyEndPos, t);
            float downwardDip = 4f * t * (1f - t) * arcHeight;
            rectTransform.position = linearPos + (Vector3.down * downwardDip);

            //flip
            float rotationY = t * 180f;
            rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
            if (t >= 0.5f && !isFlipped)
            {
                isFlipped = true;
                SwapToFaceDown();
            }

            foodImage.transform.position = Vector3.Lerp(foodStartPos, target.position, easeT);

            yield return null;
        }

        rectTransform.position = bodyEndPos;
        foodImage.transform.position = target.position;

        foodImage.transform.SetParent(target, true);
        //foodToDestroy = null;


        Image discardPileImage = discardTransform.GetComponentInChildren<Image>(true);
        if (discardPileImage != null)
        {
            discardPileImage.gameObject.SetActive(true);
        }
    }
    //State Handlers
    private void HandleIdleState()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale, Time.deltaTime * 10f);
        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, originalPosition, Time.deltaTime * 10f);
        rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, originalRotation, Time.deltaTime * 10f);
        glowEffect.SetActive(false);
    }
    private void HandleHoverState()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * hoverScale, Time.deltaTime * 10f);
        glowEffect.SetActive(true);
        glowEffect.GetComponent<UnityEngine.UI.Image>().color = hoverGlowColor;
    }
    private void HandleSelectedState()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * selectScale, Time.deltaTime * 10f);
        glowEffect.SetActive(true);
        glowEffect.GetComponent<UnityEngine.UI.Image>().color = selectedGlowColor;
    }

    private void ToggleSelected()
    {
        if ( currentState == 2)
        {
            currentState = 1;
        } else
        {
            currentState = 2;
        }
        handManager.SetSelected(this);
    }

    //Event Handlers

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentState == 3) return;
        if (Input.GetMouseButton(0))
        {
            ToggleSelected();
        }
        if (currentState == 0)
        {
            currentState = 1;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 3) return;
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState == 3) return;
        ToggleSelected();
    }

    private void OnDestroy()
    {
        if (foodToDestroy != null)
        {
            Destroy(foodToDestroy);
        }
    }
}
