using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

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

    // Colors
    private Color hoverGlowColor = Color.grey;
    private Color selectedGlowColor = Color.white;

    //
    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private float hoverScale = 1.05f;
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

    public void PlayFancyAnimation(Transform target)
    {
        TransitionToPlayedState();
        StartCoroutine(FancyAnimation(target));
    }

    private IEnumerator FancyAnimation(Transform target)
    {
        cardDisplay = GetComponent<CardDisplay>();
        Image imageToSwirl = cardDisplay.foodImage;
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        Canvas rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        imageToSwirl.transform.SetParent(rootCanvas.transform, true);
        foodToDestroy = imageToSwirl.gameObject;

        float bodyDuration = 0.5f;
        float elapsed = 0f;
        Vector3 bodyStartPos = rectTransform.localPosition;
        Vector3 bodyEndPos = bodyStartPos + Vector3.down * 1000f;

        while (elapsed < bodyDuration)
        {
            if (target == null || imageToSwirl == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / bodyDuration;
            rectTransform.localPosition = Vector3.Lerp(bodyStartPos, bodyEndPos, t);
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        canvasGroup.alpha = 0f;

        yield return new WaitForSeconds(1.0f);

        float swirldDuration = 1.25f;
        elapsed = 0f;
        Vector3 swirlStartPos = imageToSwirl.transform.position;
        float startRadius = 200f;
        float swirlSpeed = 10f;

        while (elapsed < swirldDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swirldDuration;
            float easeT = t * t * (3f - 2f * t);

            float currentRadius = Mathf.Lerp(startRadius, 0, easeT);
            float angle = elapsed * swirlSpeed;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * currentRadius;

            imageToSwirl.transform.position = Vector3.Lerp(swirlStartPos, target.position, easeT) + offset; ;
            yield return null;
        }

        imageToSwirl.transform.position = target.position;
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
