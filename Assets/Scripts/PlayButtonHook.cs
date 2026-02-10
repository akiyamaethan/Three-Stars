using UnityEngine;

public class PlayButtonHook : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager;

    public void OnPlayClicked()
    {
        if (selectionManager == null) return;
        selectionManager.PlaySelected();
    }
}
