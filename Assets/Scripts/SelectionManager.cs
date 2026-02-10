// Assets/Scripts/SelectionManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private int maxSelected = 5;

    // If you want Play to require exactly 5, set this true in Inspector.
    [SerializeField] private bool requireExactFive = false;

    private readonly List<SelectableCard> selected = new();

    // Play events
    public event Action<List<int>> OnPlayedSlotIndices;
    public event Action<List<CardData>> OnPlayedCards;

    // Discard event (slot indices only)
    public event Action<List<int>> OnDiscardSlotIndices;

    // UI can listen to this to enable/disable buttons, show counts, etc.
    public event Action<int> OnSelectionCountChanged;

    public void TrySelect(SelectableCard card)
    {
        if (card == null) return;
        if (selected.Count >= maxSelected) return;

        selected.Add(card);
        card.SetSelected(true);

        OnSelectionCountChanged?.Invoke(selected.Count);
    }

    public void Deselect(SelectableCard card)
    {
        if (card == null) return;

        if (selected.Remove(card))
        {
            card.SetSelected(false);
            OnSelectionCountChanged?.Invoke(selected.Count);
        }
    }

    public void ClearSelection()
    {
        foreach (var c in selected)
        {
            if (c != null) c.SetSelected(false);
        }
        selected.Clear();

        OnSelectionCountChanged?.Invoke(0);
    }

    public void PlaySelected()
    {
        if (selected.Count == 0) return;

        if (requireExactFive && selected.Count != 5)
        {
            // Optional: show UI feedback later
            return;
        }

        var indicesSet = new HashSet<int>();
        var indices = new List<int>();
        var cards = new List<CardData>();

        foreach (var s in selected)
        {
            if (s == null) continue;

            // Slot index (for GameManager to clear/refill exact slots)
            var slotIndex = s.transform.parent.GetComponent<SlotIndex>();
            if (slotIndex != null && indicesSet.Add(slotIndex.Index))
            {
                indices.Add(slotIndex.Index);
            }

            // Card data (for DishSpawner to display immediately)
            var view = s.GetComponent<CardView>();
            if (view != null && view.Data != null)
            {
                cards.Add(view.Data);
            }
        }

        OnPlayedCards?.Invoke(cards);
        OnPlayedSlotIndices?.Invoke(indices);

        ClearSelection();
    }

    public void DiscardSelected()
    {
        if (selected.Count == 0) return;

        var indicesSet = new HashSet<int>();
        var indices = new List<int>();

        foreach (var s in selected)
        {
            if (s == null) continue;

            var slotIndex = s.transform.parent.GetComponent<SlotIndex>();
            if (slotIndex != null && indicesSet.Add(slotIndex.Index))
            {
                indices.Add(slotIndex.Index);
            }
        }

        OnDiscardSlotIndices?.Invoke(indices);

        ClearSelection();
    }
}
