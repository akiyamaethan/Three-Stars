using UnityEngine;
using UnityEngine.UI;

public class HandHoverPreviewController : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private Vector2 offset = new Vector2(20f, -20f);

    private RectTransform previewRectTransform;
    private Canvas parentCanvas;

    private void Awake()
    {
        if (previewImage == null)
        {
            Debug.LogError("Preview Image is not assigned on HandHoverPreviewController.");
            return;
        }

        previewRectTransform = previewImage.GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        previewImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (previewImage != null && previewImage.gameObject.activeSelf)
        {
            MovePreviewNearMouse();
        }
    }

    public void ShowPreview(Sprite sprite)
    {
        if (previewImage == null || sprite == null) return;

        previewImage.sprite = sprite;
        previewImage.gameObject.SetActive(true);
        MovePreviewNearMouse();
    }

    public void HidePreview()
    {
        if (previewImage == null) return;

        previewImage.gameObject.SetActive(false);
    }

    private void MovePreviewNearMouse()
    {
        if (previewRectTransform == null || parentCanvas == null) return;

        Vector2 screenPosition = (Vector2)Input.mousePosition + offset;

        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
            out Vector2 localPoint))
        {
            previewRectTransform.localPosition = localPoint;
        }
    }
}