using System.Collections;
using System.Collections.Generic;
using ThreeStars;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ThreeStars.PlayingCard;

public class CardDisplay : MonoBehaviour
{
    public CardInstance cardInstance;

    public Image cardImage;
    public Image cardBackground;
    public Image foodImage;
    public Image rankImage;
    public Image suitImage;
    public TMP_Text cardText;
    public TMP_Text rankText;
    public TMP_Text suitText;

    private Color[] suitColors = new Color[]
    {
        new Color(1f, 0.5f, 0.5f), // Light Red
        new Color(0.5f, 0.5f, 1f), // Light Blue
        new Color(0.5f, 1f, 0.5f), // Light Green
        new Color(1f, 1f, 0.5f)    // Light Yellow
    };


    void Start()
    {
    }

    public void UpdateCardDisplay()
    { 
        var cardData = cardInstance.cardData;
        cardBackground.color = suitColors[(int)cardData.cardSuit];
        cardBackground.sprite = cardData.cardBackground;
        foodImage.sprite = cardData.foodImage;
        rankImage.sprite = cardData.rankImage;
        suitImage.sprite = cardData.suitImage;
        suitText.text = cardData.cardSuit.ToString();
        rankText.text = cardData.cardRank.ToString();
        cardText.text = cardData.cardText;
    }

}
