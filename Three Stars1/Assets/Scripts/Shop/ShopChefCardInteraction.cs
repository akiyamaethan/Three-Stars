using UnityEngine.EventSystems;
using UnityEngine;
using ThreeStars;

public class ShopChefCardInteraction : MonoBehaviour, IPointerClickHandler
{
    private ChefCard chefCard;
    private ShopManager shopManager;
    private GameObject visual;
    
    public void Setup(ChefCard data, ShopManager manager, GameObject visualCard)
    {
        chefCard = data;
        shopManager = manager;
        visual = visualCard;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopManager != null && chefCard != null)
        {
            shopManager.PurchaseChef(chefCard, visual);
        }
    }
}
