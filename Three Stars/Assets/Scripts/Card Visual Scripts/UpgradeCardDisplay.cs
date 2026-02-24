using ThreeStars;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardDisplay : MonoBehaviour
{
    public UpgradeCard cardData;
    public Image cardBackground;
    public TMP_Text cardName;
    public TMP_Text cardDescription;

    public Color[] rarityColors = new Color[]
    {
        Color.green,
        Color.cyan,
        Color.magenta,
        Color.yellow
    };

    private Color getRarityColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                return rarityColors[0];
            case CardRarity.Uncommon:
                return rarityColors[1];
            case CardRarity.Rare:
                return rarityColors[2];
            case CardRarity.Legendary:
                return rarityColors[3];
            default:
                return Color.white;
        }
    }

    public void UpdateCardDisplay()
    {
        cardBackground.color = getRarityColor(cardData.rarity);
        cardBackground.sprite = cardData.cardBackground;
        cardName.text = cardData.cardName;
        cardDescription.text = cardData.cardText;
    }

}
