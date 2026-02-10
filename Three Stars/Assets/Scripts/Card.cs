using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeStars
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class PlayingCard : ScriptableObject
    //Three types of card: PlayingCard, ChefCard, and UpgradeCard
    {
        public Sprite cardBackground;
        public Sprite foodImage;
        public Sprite rankImage;
        public Sprite suitImage;
        public string cardText;

        public enum cardSuit
        {
            Entree,
            Side,
            Vegetable,
            Sauce
        }

        public enum cardRank
        {
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace
        }
    }
}