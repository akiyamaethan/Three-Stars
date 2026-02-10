using UnityEngine;
using UnityEngine.UI;

public class ActionButtonsUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button discardButton;

    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private bool requireExactFiveToPlay = false;
    [SerializeField] private int maxSelected = 5;

    private int selectedCount = 0;

    private void OnEnable()
    {
        if (selectionManager != null)
            selectionManager.OnSelectionCountChanged += HandleSelectionChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (selectionManager != null)
            selectionManager.OnSelectionCountChanged -= HandleSelectionChanged;
    }

    private void HandleSelectionChanged(int count)
    {
        selectedCount = count;
        Refresh();
    }

    public void Refresh()
    {
        if (gameManager == null) return;

        bool canPlay = gameManager.HandsRemaining > 0 && selectedCount > 0;
        if (requireExactFiveToPlay)
            canPlay = gameManager.HandsRemaining > 0 && selectedCount == maxSelected;

        bool canDiscard = gameManager.DiscardsRemaining > 0 && selectedCount > 0;

        if (playButton != null) playButton.interactable = canPlay;
        if (discardButton != null) discardButton.interactable = canDiscard;
    }
}