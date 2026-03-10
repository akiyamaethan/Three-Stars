using System.Collections.Generic;
using UnityEngine;

namespace ThreeStars
{
    [CreateAssetMenu(fileName = "New Upgrade Card", menuName = "Upgrade Card")]
    public class UpgradeCard : ScriptableObject
    {
        public Sprite cardBackground;
        public Sprite cardImage;
        public string cardText;
        public string cardName;
        public List<UpgradeCategory> upgradeCategories;
        public float effectMagnitude;
        public CardRarity rarity;
    }
}
