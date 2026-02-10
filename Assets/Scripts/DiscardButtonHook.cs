using UnityEngine;

public class DiscardButtonHook : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;

    public void OnDiscardClicked()
    {
        if (selectionManager == null) return;
        selectionManager.DiscardSelected();
    }
}
