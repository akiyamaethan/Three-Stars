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
        if (cardModalRoot == null)
        {
            cardModalRoot = GameObject.Find("CardModalRoot");
        }

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

    public void ToggleModal()
    {
        if (cardModalRoot == null) return;
        isOpen = !isOpen;
        cardModalRoot.SetActive(isOpen);
    }
}
