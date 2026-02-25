using ThreeStars;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject upgradeCardPrefab;
    public Transform shopContentParent;
    public List<UpgradeCard> availableUpgrades;

    private void Awake()
    {
        var loaded = new List<UpgradeCard>(Resources.LoadAll<UpgradeCard>("Upgrade Card Data"));
        foreach (var upgradeCard in loaded)
        {
            if (upgradeCard != null) availableUpgrades.Add(upgradeCard);
        }
        Debug.Log($"Loaded {availableUpgrades.Count} Upgrade cards");
        
    }
    public void OpenShop()
    {
        ClearShop();
        GenerateCards(3);
    }

    public void ClearShop()
    {
        foreach (Transform child in shopContentParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void GenerateCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            UpgradeCard randomUpgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
            GameObject cardGO = Instantiate(upgradeCardPrefab, shopContentParent);
            UpgradeCardDisplay cardDisplay = cardGO.GetComponent<UpgradeCardDisplay>();
            cardDisplay.cardData = randomUpgrade;
            cardDisplay.UpdateCardDisplay();

            Button btn = cardGO.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(() => PurchaseUpgrade(randomUpgrade, cardGO));
        }
    }

    public void PurchaseUpgrade(UpgradeCard upgrade, GameObject visual)
    {
        GameManager.Instance.progressionManager.ApplyUpgrade(upgrade);
        Destroy(visual);
    }
}
