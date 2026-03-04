using UnityEngine.EventSystems;
using UnityEngine;
using ThreeStars;

public class ShopCardInteraction : MonoBehaviour, IPointerClickHandler
{
    private UpgradeCard upgradeCard;
    private ShopManager shopManager;
    private GameObject visual;
    
    public void Setup(UpgradeCard data, ShopManager manager, GameObject visualCard)
    {
        upgradeCard = data;
        shopManager = manager;
        visual = visualCard;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopManager != null && upgradeCard != null)
        {
            shopManager.PurchaseUpgrade(upgradeCard, visual);
        }
    }
}
