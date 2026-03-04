using UnityEngine;
using System.Collections.Generic;

namespace ThreeStars
{
    public enum ChefEffectType
    {
        AdditivePips, Multiplier, DrawModifier, GameSpeed, Special
    }

    [CreateAssetMenu(fileName = "New Chef Card", menuName = "Chef Card")]
    public class ChefCard : ScriptableObject
    {
        public string cardName;
        public ChefEffectType effectType;
        public float effectMagnitude;
        public int durability;

        public PlayingCard.CardRank targetRankLow;
        public PlayingCard.CardRank targetRankHigh;
        public PlayingCard.CardSuit targetSuit;
        public HandEvaluator.HandRank targetHand;

        public Sprite cardBackground;
        public Sprite cardImage;
        public string cardText;
        public string description;
        public string effect;
        public CardRarity rarity;
    }
}
