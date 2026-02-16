using UnityEngine;

public class CardModalController : MonoBehaviour
{
    [Header("Modal Root")]
    [SerializeField] private GameObject cardModalRoot;

    [Header("Input")]
    [SerializeField] private KeyCode toggleKey = KeyCode.M;

    private bool isOpen = false;

    void Start()
    {
        if (cardModalRoot != null)
        {
            cardModalRoot.SetActive(false);
            isOpen = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleModal();
        }
    }

    private void ToggleModal()
    {
        isOpen = !isOpen;
        cardModalRoot.SetActive(isOpen);
    }
}
