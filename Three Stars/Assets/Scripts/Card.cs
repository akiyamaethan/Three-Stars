using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeStars
{
    [CreateAssetMenu(fileName = "New Playing Card", menuName = "Playing Card")]
    public class PlayingCard : ScriptableObject
    //Three types of card: PlayingCard, ChefCard, and UpgradeCard
    {
        public Sprite cardBackground;
        public Sprite foodImage;
        public Sprite rankImage;
        public Sprite suitImage;
        public string cardText;
        public CardSuit cardSuit;
        public CardRank cardRank;

        public enum CardSuit
        {
            Entree,
            Side,
            Vegetable,
            Sauce
        }

        public enum CardRank
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

    [CreateAssetMenu(fileName = "New Chef Card", menuName = "Chef Card")]
    public class ChefCard : ScriptableObject
    //Three types of card: PlayingCard, ChefCard, and UpgradeCard
    {
        public Sprite cardBackground;
        public Sprite cardImage;
        public string cardText;
        public string description;
    }

    [CreateAssetMenu(fileName = "New Upgrade Card", menuName = "Upgrade Card")]
    public class UpgradeCard : ScriptableObject
    //Three types of card: PlayingCard, ChefCard, and UpgradeCard
    {
        public Sprite cardBackground;
        public Sprite cardImage;
        public string cardText;
        public string description;
    }
}