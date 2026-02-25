using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopModalRoot : MonoBehaviour
{
    [Header("Wiring")]
    [SerializeField] private Button closeButton;

    // Optional: if you have a dimmer and want it clickable to close too
    [SerializeField] private Button dimmerButton;

    // Event fired when the player closes the shop
    public event Action Closed;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (dimmerButton != null)
            dimmerButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Closed?.Invoke();
    }
}