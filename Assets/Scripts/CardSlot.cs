using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var droppedGO = eventData.pointerDrag;
        if (droppedGO == null) return;

        var droppedCard = droppedGO.GetComponent<DraggableCard>();
        if (droppedCard == null) return;

        // If this slot already has a card, swap it back to the dropped card's original parent
        if (transform.childCount > 0)
        {
            var existing = transform.GetChild(0);
            existing.SetParent(droppedCard.OriginalParent, false);

            var existingDrag = existing.GetComponent<DraggableCard>();
            if (existingDrag != null) existingDrag.SnapToFillParent();
        }

        // Place dropped card into this slot
        droppedGO.transform.SetParent(transform, false);
        droppedCard.SnapToFillParent();
    }
}
