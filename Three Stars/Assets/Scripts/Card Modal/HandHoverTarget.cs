using UnityEngine;
using UnityEngine.EventSystems;

public class HandHoverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private HandHoverPreviewController previewController;
    [SerializeField] private Sprite previewSprite;
    private string handDescription;

    public void Setup(HandHoverPreviewController controller, Sprite sprite, string description)
    {
        previewController = controller;
        previewSprite = sprite;
        handDescription = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (previewController != null && previewSprite != null)
        {
            previewController.ShowPreview(previewSprite, handDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (previewController != null)
        {
            previewController.HidePreview();
        }
    }

    private void OnDisable()
    {
        if (previewController != null ) { previewController.HidePreview(); }
    }
}