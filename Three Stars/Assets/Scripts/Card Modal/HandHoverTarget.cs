using UnityEngine;
using UnityEngine.EventSystems;

public class HandHoverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HandHoverPreviewController previewController;
    [SerializeField] private Sprite previewSprite;

    public void Setup(HandHoverPreviewController controller, Sprite sprite)
    {
        previewController = controller;
        previewSprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (previewController != null && previewSprite != null)
        {
            previewController.ShowPreview(previewSprite);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (previewController != null)
        {
            previewController.HidePreview();
        }
    }
}