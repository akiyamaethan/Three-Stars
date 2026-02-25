using ThreeStars;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject upgradeCardPrefab;
    public Transform shopContentParent;
    public List<UpgradeCard> availableUpgrades;
    public List<GameObject> visibleUpgrades;
    public int cardSpacing = 250;

    public ProgressionManager progressionManager;

    public static event System.Action OnShopUIUpdate;

    private void Awake()
    {
        availableUpgrades = new List<UpgradeCard>();
        visibleUpgrades = new List<GameObject>();

        var loaded = new List<UpgradeCard>(Resources.LoadAll<UpgradeCard>("Upgrade Card Data"));
        foreach (var upgradeCard in loaded)
        {
            if (upgradeCard != null) availableUpgrades.Add(upgradeCard);
        }
        Debug.Log($"Loaded {availableUpgrades.Count} Upgrade cards");
        
    }

    public void Start()
    {
        progressionManager = GameManager.Instance.progressionManager;
    }
    public void OpenShop()
    {
        ClearShop();
        GenerateCards(3);
        OnShopUIUpdate?.Invoke();
    }

    public void ClearShop()
    {
        foreach (Transform child in shopContentParent)
        {
            Destroy(child.gameObject);
        }
        visibleUpgrades.Clear();
    }

    private void GenerateCards(int count)
    {
        visibleUpgrades.Clear();
        for (int i = 0; i < count; i++)
        {
            UpgradeCard randomUpgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
            GameObject cardGO = Instantiate(upgradeCardPrefab, shopContentParent);
            visibleUpgrades.Add(cardGO);

            UpgradeCardDisplay cardDisplay = cardGO.GetComponent<UpgradeCardDisplay>();
            cardDisplay.cardData = randomUpgrade;
            cardDisplay.UpdateCardDisplay();

            ShopCardInteraction interaction = cardGO.GetComponent<ShopCardInteraction>();
            interaction.Setup(randomUpgrade, this, cardGO);
        }

        if(visibleUpgrades.Count > 0)
        {
            float totalWidth = (visibleUpgrades.Count - 1) * cardSpacing;
            float startX = -totalWidth / 2;
            for (int i = 0; i < visibleUpgrades.Count; i++)
            {
                Vector3 targetPosition = new Vector3(startX + i * cardSpacing, 0, 0);
                visibleUpgrades[i].transform.localPosition = targetPosition;
            }
        }

    }

    public void PurchaseUpgrade(UpgradeCard upgrade, GameObject visual)
    {
        int price;
        switch (upgrade.rarity)
        {
            case CardRarity.Common:
                price = 5;
                break;
            case CardRarity.Uncommon:
                price = 7;
                break;
            case CardRarity.Rare:
                price = 9;
                break;
            case CardRarity.Legendary:
                price = 12;
                break;
            default:
                price = 3;
                break;
        }
        if (progressionManager.playerBalance >= price)
        {
            progressionManager.playerBalance -= price;
        } else
        {
            return;
        }
        progressionManager.ApplyUpgrade(upgrade);
        Destroy(visual);
        OnShopUIUpdate?.Invoke();
    }
}
