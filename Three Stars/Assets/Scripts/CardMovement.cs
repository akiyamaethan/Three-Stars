using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private int currentState = 0;
    private Quaternion originalRotation;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlay;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private GameObject glowEffect;
    //[SerializeField] private GameObject playArrow;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
    }

    void Update()
    {
        switch (currentState)
        {
            case 0: // Idle
                break;
            case 1: // Hovered
                HandleHoverState();
                break;
            case 2: //Selected
                HandleSelectedState();
                break;
            case 3: // Played
                HandlePlayedState();
                break;
        }
    }

    private void TransitionToState0()
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
        glowEffect.SetActive(false);
        //playArrow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentState == 0)
        {
            currentState = 1;
        }
    }
    private void HandleHoverState()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * selectScale, Time.deltaTime * 10f);
        glowEffect.SetActive(true);
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            originalPanelLocalPosition = rectTransform.localPosition;
        }
    }

    private void HandleSelectedState()
    {
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, originalScale * selectScale, Time.deltaTime * 10f);
        glowEffect.SetActive(true);
    }

    private void HandlePlayedState()
    {
        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, playPosition, Time.deltaTime * 10f);
        rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 10f);
        if (Vector3.Distance(rectTransform.localPosition, playPosition) < 0.1f)
        {
            rectTransform.localPosition = playPosition;
            rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            //playArrow.SetActive(true);
        }
    }
}
