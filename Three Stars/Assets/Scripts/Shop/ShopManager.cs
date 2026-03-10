using ThreeStars;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ShopManager : MonoBehaviour
{
    public GameObject upgradeCardPrefab;
    public GameObject chefCardPrefab;
    public Transform UpgradeTransform;
    public Transform ChefTransform;
    public List<UpgradeCard> availableUpgrades;
    public List<ChefCard> availableChefs;
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
        

        availableChefs = new List<ChefCard>();
        var chefs = new List<ChefCard>(Resources.LoadAll<ChefCard>("Chef Card Data"));
        foreach(var chefCard in chefs)
        {
            if (chefCard != null) availableChefs.Add(chefCard);
        }
    }

    public void Start()
    {
        progressionManager = GameManager.Instance.progressionManager;
    }
    public void OpenShop()
    {
        ClearShop();
        GenerateCards(3, 2);
        OnShopUIUpdate?.Invoke();
    }

    public void ClearShop()
    {
        if (UpgradeTransform != null)
        {
            foreach (Transform child in UpgradeTransform)
                Destroy(child.gameObject);
        }
        if (ChefTransform != null)
        {
            foreach (Transform child in ChefTransform)
                Destroy(child.gameObject);
        }
        visibleUpgrades.Clear();
    }

    private void GenerateCards(int upgradeCount, int chefCount)
    {
        visibleUpgrades.Clear();
        List<GameObject> currentUpgrades = new List<GameObject>();
        List<GameObject> currentChefs = new List<GameObject>();

        // Generate Upgrades
        for (int i = 0; i < upgradeCount; i++)
        {
            UpgradeCard randomUpgrade = availableUpgrades[Random.Range(0, availableUpgrades.Count)];
            GameObject cardGO = Instantiate(upgradeCardPrefab, UpgradeTransform);
            currentUpgrades.Add(cardGO);
            visibleUpgrades.Add(cardGO);

            UpgradeCardDisplay cardDisplay = cardGO.GetComponent<UpgradeCardDisplay>();
            cardDisplay.cardData = randomUpgrade;
            cardDisplay.UpdateCardDisplay();

            ShopCardInteraction interaction = cardGO.GetComponent<ShopCardInteraction>();
            interaction.Setup(randomUpgrade, this, cardGO);
        }
        LayoutGroup(UpgradeTransform, currentUpgrades);


        // Generate Chefs
        for (int i = 0; i < chefCount; i++)
        {
            if (availableChefs.Count == 0) break;
            ChefCard randomChef = availableChefs[Random.Range(0, availableChefs.Count)];
            GameObject cardGO = Instantiate(chefCardPrefab, ChefTransform);
            currentChefs.Add(cardGO);
            visibleUpgrades.Add(cardGO);

            ChefCardDisplay chefCardDisplay = cardGO.GetComponent<ChefCardDisplay>();
            chefCardDisplay.cardData = randomChef;
            chefCardDisplay.UpdateShopDisplay();

            ShopChefCardInteraction interaction = cardGO.AddComponent<ShopChefCardInteraction>();
            interaction.Setup(randomChef, this, cardGO);
        }
        LayoutGroup(ChefTransform, currentChefs);

    }

    public void PurchaseChef(ChefCard chef, GameObject visual)
    {
        if (progressionManager.AddChef(chef))
        {
            Destroy(visual);
            visibleUpgrades.Remove(visual);
            OnShopUIUpdate?.Invoke();
        }
    }

    private void LayoutGroup(Transform parent, List<GameObject> group)
    {
        if (group.Count == 0) return;
        float totalWidth = (group.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;
        for (int i = 0; i < group.Count; i++)
        {
            group[i].transform.localPosition = new Vector3(startX + i * cardSpacing, 0, 0);
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
