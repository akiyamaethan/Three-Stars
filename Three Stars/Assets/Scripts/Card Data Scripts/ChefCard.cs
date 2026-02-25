using UnityEngine;

namespace ThreeStars
{
    [CreateAssetMenu(fileName = "New Chef Card", menuName = "Chef Card")]
    public class ChefCard : ScriptableObject
    {
        public Sprite cardBackground;
        public Sprite cardImage;
        public string cardText;
        public string description;
        public string effect;
        public CardRarity rarity;
    }
}
