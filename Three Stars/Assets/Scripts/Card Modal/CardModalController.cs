using UnityEngine;

public class CardModalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cardModalRoot;
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    private CardModalUI cardModalUI;

    private void Awake()
    {
        if (cardModalRoot != null)
        {
            cardModalUI = cardModalRoot.GetComponent<CardModalUI>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleModal();
        }
    }

    public void ToggleModal()
    {
        if (cardModalRoot == null)
        {
            Debug.LogWarning("CardModalController: cardModalRoot is not assigned.");
            return;
        }

        bool shouldOpen = !cardModalRoot.activeSelf;

        if (shouldOpen && cardModalUI != null)
        {
            cardModalUI.Refresh();
        }

        cardModalRoot.SetActive(shouldOpen);
    }

    public void CloseModal()
    {
        if (cardModalRoot != null)
        {
            cardModalRoot.SetActive(false);

            if (cardModalUI != null && cardModalUI.hoverController != null)
            {
                cardModalUI.hoverController.HidePreview();
            }
        }
    }
}