using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Quaternion originalRotation;
    private bool isHovered = false;

    [SerializeField] private float tiltAngle = 10f;
    [SerializeField] private float lerpSpeed = 10f;

    [Header("Idle Wobble")]
    [SerializeField] private float idleWobbleIntensity = 3f;
    [SerializeField] private float idleWobbleSpeed = 0.5f;
    private float noiseOffsetX;
    private float noiseOffsetY;

    // To prevent fighting with animations in CardMovement
    public bool isSuppressed = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalRotation = rectTransform.localRotation;

        // Randomize offsets so cards don't wobble in perfect sync
        noiseOffsetX = Random.value * 100f;
        noiseOffsetY = Random.value * 100f;
    }

    void Update()
    {
        if (isSuppressed) return;

        Quaternion targetRotation;

        if (isHovered)
        {
            targetRotation = CalculateTilt();
        }
        else
        {
            targetRotation = CalculateIdleWobble();
        }

        rectTransform.localRotation = Quaternion.Slerp(rectTransform.localRotation, targetRotation, Time.deltaTime * lerpSpeed);
    }

    private Quaternion CalculateTilt()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint);

        float normalizedX = Mathf.Clamp(localPoint.x / (rectTransform.rect.width * 0.5f), -1f, 1f);
        float normalizedY = Mathf.Clamp(localPoint.y / (rectTransform.rect.height * 0.5f), -1f, 1f);

        return originalRotation * Quaternion.Euler(-normalizedY * tiltAngle, normalizedX * tiltAngle, 0f);
    }

    private Quaternion CalculateIdleWobble()
    {
        // Use Perlin noise to create smooth, random-feeling motion
        float time = Time.time * idleWobbleSpeed;
        float noiseX = Mathf.PerlinNoise(noiseOffsetX + time, 0f) * 2f - 1f; // Range -1 to 1
        float noiseY = Mathf.PerlinNoise(0f, noiseOffsetY + time) * 2f - 1f; // Range -1 to 1

        return originalRotation * Quaternion.Euler(noiseY * idleWobbleIntensity, noiseX * idleWobbleIntensity, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void SetOriginalRotation(Quaternion rot)
    {
        originalRotation = rot;
    }
}
