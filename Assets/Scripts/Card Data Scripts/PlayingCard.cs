using UnityEngine;

namespace ThreeStars
{
    [CreateAssetMenu(fileName = "New Playing Card", menuName = "Playing Card")]
    public class PlayingCard : ScriptableObject
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
}
