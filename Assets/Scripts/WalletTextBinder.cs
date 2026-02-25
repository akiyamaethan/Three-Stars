using TMPro;
using UnityEngine;

public class WalletTextBinder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string prefix = "Wallet: ";

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        ShopManager.OnWalletChanged += OnWalletChanged;

        // Refresh immediately
        if (ShopManager.Instance != null)
            OnWalletChanged(ShopManager.Instance.Wallet);
        else
            OnWalletChanged(0);
    }

    private void OnDisable()
    {
        ShopManager.OnWalletChanged -= OnWalletChanged;
    }

    private void OnWalletChanged(int amount)
    {
        if (text != null)
            text.text = $"{prefix}{amount}";
    }
}