using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject highlight; // optional child object (outline/glow)
    public bool IsSelected { get; private set; }

    private SelectionManager selectionManager;

    public void Init(SelectionManager manager)
    {
        selectionManager = manager;
        SetSelected(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectionManager == null) return;

        if (IsSelected)
        {
            selectionManager.Deselect(this);
        }
        else
        {
            selectionManager.TrySelect(this);
        }
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (highlight != null)
        {
            highlight.SetActive(selected);
        }
        else
        {
            // fallback: slightly scale up when selected
            transform.localScale = selected ? new Vector3(1.05f, 1.05f, 1f) : Vector3.one;
        }
    }
}
