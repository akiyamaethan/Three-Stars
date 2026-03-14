using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandHoverPreviewController : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private Vector2 offset = new Vector2(0f, -100f);

    private RectTransform rootRectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        if (previewImage == null)
        {
            Debug.LogError("Preview Image is not assigned on HandHoverPreviewController.");
            return;
        }

        rootRectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        var bgImage = GetComponent<Image>();
        if (bgImage != null) bgImage.raycastTarget = false;

        if (previewImage != null) previewImage.raycastTarget = false;
        if (descriptionText != null) descriptionText.raycastTarget = false;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            MovePreviewNearMouse();
        }
    }

    public void ShowPreview(Sprite sprite, string description)
    {
        if (previewImage == null || sprite == null) return;
        previewImage.sprite = sprite;
        previewImage.SetNativeSize();

        previewImage.transform.localScale = Vector3.one * 1.5f;
        descriptionText.transform.localScale = Vector3.one;

        previewImage.rectTransform.localPosition = new Vector3(previewImage.rectTransform.localPosition.x,
            previewImage.rectTransform.localPosition.y, 0);
        descriptionText.rectTransform.localPosition = new Vector3(descriptionText.rectTransform.localPosition.x,
            descriptionText.rectTransform.localPosition.y, 0);

        previewImage.gameObject.SetActive(true);
        if (description != null)
        {
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = description; 
        }
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        Canvas.ForceUpdateCanvases();
        MovePreviewNearMouse();
    }

    public void HidePreview()
    {
        gameObject.SetActive(false);
    }

    private void MovePreviewNearMouse()
    {
        if (rootRectTransform == null || parentCanvas == null) return;

        Vector2 screenPosition = (Vector2)Input.mousePosition + offset;
        RectTransform parentRect = rootRectTransform.parent as RectTransform;

        if (parentRect == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
            out Vector2 localPoint))
        {
            rootRectTransform.localPosition = localPoint;
        }
    }
}