using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas rootCanvas;
    private RectTransform rt;
    private CanvasGroup canvasGroup;

    public Transform OriginalParent { get; private set; }
    private Vector2 originalAnchoredPos;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OriginalParent = transform.parent;
        originalAnchoredPos = rt.anchoredPosition;

        // Put on top while dragging
        transform.SetParent(rootCanvas.transform, true);

        // Allow slots to receive drop
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // If still under the canvas, it means no slot accepted it
        if (transform.parent == rootCanvas.transform)
        {
            ReturnToOriginalParent();
        }
    }

    public void ReturnToOriginalParent()
    {
        transform.SetParent(OriginalParent, false);
        SnapToFillParent();
        rt.anchoredPosition = originalAnchoredPos;
    }

    public void SnapToFillParent()
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }
}