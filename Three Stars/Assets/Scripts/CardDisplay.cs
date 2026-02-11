using System.Collections;
using System.Collections.Generic;
using ThreeStars;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ThreeStars.PlayingCard;

public class CardDisplay : MonoBehaviour
{
    public PlayingCard cardData;

    public Image cardImage;
    public Image cardBackground;
    public Image foodImage;
    public Image rankImage;
    public Image suitImage;
    public TMP_Text cardText;
    public TMP_Text rankText;
    public TMP_Text suitText;



    void Start()
    {
    }

    public void UpdateCardDisplay()
    { 
        cardBackground.sprite = cardData.cardBackground;
        foodImage.sprite = cardData.foodImage;
        rankImage.sprite = cardData.rankImage;
        suitImage.sprite = cardData.suitImage;
        suitText.text = cardData.cardSuit.ToString();
        rankText.text = cardData.cardRank.ToString();
        cardText.text = cardData.cardText;
    }

}
