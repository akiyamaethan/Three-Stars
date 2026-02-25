using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private RectTransform discardTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private int currentState = 0;
    private Quaternion originalRotation;
    private Coroutine animationCoroutine;
    private HandManager handManager;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlay;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private float moveDuration = 1.0f;

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
        Vector3 newPos = discardTransform.localPosition;
        AnimateTo(newPos);
    }

    public void Play()
    {
        TransitionToPlayedState();
        Vector3 newPos = discardTransform.localPosition;
        AnimateTo(newPos);
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

    private IEnumerator AnimatePosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.localPosition;

        // If we're already very close, just snap to the target
        if (Vector3.Distance(startPosition, targetPosition) < 0.01f)
        {
            transform.localPosition = targetPosition;
            yield break;
        }

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;
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
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * selectScale, Time.deltaTime * 10f);
        glowEffect.SetActive(true);
    }
    private void HandleSelectedState()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * selectScale, Time.deltaTime * 10f);
        glowEffect.SetActive(true);

    }

    //Event Handlers

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentState == 0)
        {
            currentState = 1;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            currentState = 2;
            handManager.SetSelected(this);
        }

        else if (currentState == 2)
        {
            currentState = 1;
            handManager.SetSelected(this);
        }
    }
}
